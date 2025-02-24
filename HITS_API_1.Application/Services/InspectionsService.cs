using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Exceptions;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Exceptions;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class InspectionsService(
    IInspectionsRepository inspectionsRepository,
    IDiagnosesRepository diagnosesRepository,
    IConsultationsService consultationsService,
    IPatientsRepository patientsRepository,
    IDoctorsRepository doctorsRepository,
    IDiagnosesService diagnosesService,
    IIcd10Repository icd10Repository,
    IConsultationsRepository consultationsRepository,
    IPaginationService paginationService,
    IIcd10Service icd10Service,
    GetFilteredInspectionsRequestValidator getFilteredInspectionsRequestValidator,
    RedactInspectionRequestValidator redactInspectionRequestValidator,
    CreateInspectionRequestValidator createInspectionRequestValidator)
    : IInspectionsService
{
    public async Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId)
    {
        var validationResult = await createInspectionRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var patient = await patientsRepository.GetById(patientId);
        if (patient == null)
        {
            throw new NotFoundObjectException("patient", "Пациент не найден");
        }

        await diagnosesService.ValidateDiagnoses(request.diagnoses);

        if (request.consultations != null)
        {
            await consultationsService.ValidateConsultations(request.consultations);
        }

        // Валидация по предыдущим осмотрам
        var patientInspections = await inspectionsRepository.GetAllByPatientId(patientId);

        var previousInspectionExists = false;

        foreach (var patientInspection in patientInspections)
        {
            if (request.previousInspectionId != null)
            {
                if (request.previousInspectionId == patientInspection.Id)
                {
                    if (patientInspection.Conclusion == Conclusion.Death)
                    {
                        throw new IncorrectFieldException("previousInspectionId",
                            "Осмотр не может ссылаться на осмотр с заключением \"Смерть\"");
                    }

                    if (request.date < patientInspection.Date)
                    {
                        throw new IncorrectFieldException("previousInspectionId",
                            "Дата осмотра не может быть раньше даты предыдущего осмотра");
                    }

                    var childInspection = await inspectionsRepository.GetByParentInspectionId(patientInspection.Id);
                    if (childInspection != null)
                    {
                        throw new IncorrectFieldException("previousInspectionId",
                            "У этого осмотра уже есть дочерний осмотр");
                    }

                    previousInspectionExists = true;
                }
            }

            if (patientInspection.Conclusion == Conclusion.Death)
            {
                if (request.conclusion == Conclusion.Death)
                {
                    throw new IncorrectFieldException("conclusion", "Пациент уже умер");
                }

                if (patientInspection.Date < request.date)
                {
                    throw new IncorrectFieldException("date",
                        "Нельзя поставить дату осмотра позже даты смерти пациента");
                }
            }

            if (request.conclusion == Conclusion.Death && request.deathDate < patientInspection.Date)
            {
                throw new IncorrectFieldException("deathDate",
                    "Дата смерти не может быть раньше даты какого-либо другого осмотра");
            }
        }

        if (request.previousInspectionId != null && !previousInspectionExists)
        {
            throw new IncorrectFieldException("previousInspectionId", "Предыдущий осмотр не найден");
        }

        var inspection = new Inspection(request.date, request.anamnesis, request.complaints, request.treatment,
            request.conclusion, request.nextVisitDate, request.deathDate, request.previousInspectionId,
            patientId, doctorId);

        await inspectionsRepository.Create(inspection);

        foreach (var diagnosis in request.diagnoses)
        {
            var item = new Diagnosis(diagnosis.description, diagnosis.type, inspection.Id,
                diagnosis.icdDiagnosisId);
            await diagnosesRepository.Create(item);
        }

        if (request.consultations != null && request.consultations.Count > 0)
        {
            foreach (var consultation in request.consultations)
            {
                await consultationsService.CreateConsultation(inspection.Id, consultation.specialityId,
                    inspection.DoctorId, consultation.comment.content);
            }
        }

        return inspection.Id;
    }

    public async Task<GetInspectionResponse> GetInspectionById(Guid inspectionId)
    {
        var inspection = await inspectionsRepository.GetById(inspectionId);
        if (inspection == null)
        {
            throw new NotFoundObjectException("inspection", "Осмотр не найден");
        }

        var baseInspection = await GetBaseInspection(inspection);

        var patient = await patientsRepository.GetById(inspection.PatientId);
        GetPatientByIdResponse patientResponse = new GetPatientByIdResponse(patient.Id, patient.CreateTime,
            patient.Name, patient.Birthday, patient.Sex);

        var doctor = await doctorsRepository.GetById(inspection.DoctorId);
        GetDoctorResponse doctorResponse = new GetDoctorResponse(doctor.Id, doctor.CreateTime, doctor.Name,
            doctor.Birthday, doctor.Sex, doctor.Email, doctor.Phone);

        var diagnoses = await diagnosesService.GetDiagnosesByInspection(inspection.Id);
        if (diagnoses.Count == 0)
        {
            diagnoses = null;
        }

        var consultations = await
            consultationsService.GetAllConsultationsByInspection(inspection.Id);
        if (consultations.Count == 0)
        {
            consultations = null;
        }

        GetInspectionResponse response = new GetInspectionResponse(inspection.Id, inspection.CreateTime,
            inspection.Date, inspection.Anamnesis, inspection.Complaints, inspection.Treatment, inspection.Conclusion,
            inspection.NextVisitDate, inspection.DeathDate, baseInspection?.Id, inspection.PreviousInspectionId,
            patientResponse, doctorResponse, diagnoses, consultations);

        return response;
    }

    public async Task UpdateInspection(Guid inspectionId, RedactInspectionRequest request, Guid doctorId)
    {
        var validationResult = await redactInspectionRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        await diagnosesService.ValidateDiagnoses(request.diagnoses);

        var inspection = await inspectionsRepository.GetById(inspectionId);
        if (inspection == null)
        {
            throw new NotFoundObjectException("inspection", "Осмотр не найден");
        }

        if (inspection.DoctorId != doctorId)
        {
            throw new ForbiddenOperationException("Пользователь не может редактировать этот осмотр");
        }

        if (request.nextVisitDate != null && inspection.Date > request.nextVisitDate)
        {
            throw new IncorrectFieldException("nextVisitDate", "Дата следующего визита не может быть раньше даты осмотра");
        }

        var patientInspections = await inspectionsRepository.GetAllByPatientId(inspection.PatientId);

        foreach (var patientInspection in patientInspections)
        {
            if (patientInspection.Id != inspection.Id)
            {
                if (request.conclusion == Conclusion.Death)
                {
                    if (patientInspection.Conclusion == Conclusion.Death)
                    {
                        throw new IncorrectFieldException("conclusion", "Пациент уже умер");
                    }

                    if (request.deathDate < patientInspection.Date)
                    {
                        throw new IncorrectFieldException("deathDate",
                            "Дата смерти не может быть раньше даты какого-либо другого осмотра");
                    }
                }
            }
            else if (request.deathDate != null && request.deathDate > patientInspection.Date)
            {
                throw new IncorrectFieldException("deathDate", "Дата смерти не может быть позже даты осмотра");
            }
        }

        await inspectionsRepository.Update(inspection.Id, request.anamnesis, request.complaints, request.treatment,
            request.conclusion, request.nextVisitDate, request.deathDate);

        await diagnosesRepository.DeleteByInspectionId(inspection.Id);

        foreach (var diagnosisRequest in request.diagnoses)
        {
            Diagnosis diagnosis = new Diagnosis(diagnosisRequest.description, diagnosisRequest.type, inspection.Id,
                diagnosisRequest.icdDiagnosisId);

            await diagnosesRepository.Create(diagnosis);
        }
    }

    public async Task<List<GetInspectionByRootResponse>> GetInspectionsByRoot(Guid rootId)
    {
        var rootInspection = await inspectionsRepository.GetById(rootId);
        if (rootInspection == null)
        {
            throw new NotFoundObjectException("rootInspection", "Корневой осмотр не найден");
        }

        if (rootInspection.PreviousInspectionId != null)
        {
            throw new IncorrectFieldException("rootInspection", "Осмотр не является корневым");
        }

        List<GetInspectionByRootResponse> children = new List<GetInspectionByRootResponse>();

        var child = await inspectionsRepository.GetByParentInspectionId(rootInspection.Id);

        while (child != null)
        {
            var doctor = await doctorsRepository.GetById(child.DoctorId);
            var patient = await patientsRepository.GetById(child.PatientId);

            var diagnoses = await diagnosesService.GetDiagnosesByInspection(child.Id);

            var mainDiagnosis = diagnoses.FirstOrDefault(d => d.diagnosisType == DiagnosisType.Main);

            var nextChild = await inspectionsRepository.GetByParentInspectionId(child.Id);

            bool hasNested = nextChild != null;

            bool hasChain = !(!hasNested && child.PreviousInspectionId == null);

            GetInspectionByRootResponse childResponse = new GetInspectionByRootResponse(child.Id, child.CreateTime,
                child.PreviousInspectionId, child.Date, child.Conclusion, child.DoctorId, doctor.Name, child.PatientId,
                patient.Name, mainDiagnosis, hasChain, hasNested);

            children.Add(childResponse);

            child = nextChild;
        }

        return children;
    }

    public async Task<List<GetPatientInspectionsNoChildrenResponse>?> GetPatientInspectionsNoChildren(
        Guid patientId, String? filter)
    {
        var patient = await patientsRepository.GetById(patientId);
        if (patient == null)
        {
            throw new NotFoundObjectException("patient", "Пациент не найден");
        }

        var response = new List<GetPatientInspectionsNoChildrenResponse>();

        var inspections = await inspectionsRepository.GetAllByPatientId(patientId);
        if (inspections.Count == 0)
        {
            return response;
        }

        inspections = inspections.Where(i => i.PreviousInspectionId == null).ToList();
        foreach (var inspection in inspections)
        {
            var diagnoses = await diagnosesService.GetDiagnosesByInspection(inspection.Id);

            var mainDiagnosis = diagnoses
                .FirstOrDefault(d => d.diagnosisType == DiagnosisType.Main);

            if (string.IsNullOrEmpty(filter) || mainDiagnosis.code.ToLower().Contains(filter.ToLower()) ||
                mainDiagnosis.name.ToLower().Contains(filter.ToLower()))
            {
                var responseElement = new GetPatientInspectionsNoChildrenResponse(
                    inspection.Id, inspection.CreateTime, inspection.Date, mainDiagnosis);

                response.Add(responseElement);
            }
        }

        return response;
    }

    public async Task<(List<GetInspectionByRootResponse>, Pagination)> GetInspectionsForConsultation(Guid doctorId,
        GetFilteredInspectionsRequest request)
    {
        var validationResult = await getFilteredInspectionsRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        if (request.icdRoots != null && request.icdRoots.Count != 0)
        {
            await icd10Service.ValidateIcdRoots(request.icdRoots);
        }

        var doctor = await doctorsRepository.GetById(doctorId);
        if (doctor == null)
        {
            throw new KeyNotFoundException("Пользователь не найден");
        }

        var inspections = await inspectionsRepository.GetAll();
        var consultations = await consultationsRepository.GetAll();

        inspections = inspections.Where(i => consultations
                .Any(c => c.InspectionId == i.Id && c.SpecialityId == doctor.Speciality))
            .ToList();

        var response = await FilterAndPaginateInspections(request.grouped,
            request.icdRoots, request.page, request.size, inspections);
        return response;
    }

    public async Task<(List<GetInspectionByRootResponse>, Pagination)> GetPatientInspections(Guid patientId,
        GetFilteredInspectionsRequest request)
    {
        var validationResult = await getFilteredInspectionsRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        if (request.icdRoots != null && request.icdRoots.Count != 0)
        {
            await icd10Service.ValidateIcdRoots(request.icdRoots);
        }

        var patient = await patientsRepository.GetById(patientId);
        if (patient == null)
        {
            throw new NotFoundObjectException("patient", "Пациент не найден");
        }

        var inspections = await inspectionsRepository.GetAllByPatientId(patient.Id);

        var response = await FilterAndPaginateInspections(request.grouped,
            request.icdRoots, request.page, request.size, inspections);
        return response;
    }

    private async Task<Inspection?> GetBaseInspection(Inspection inspection)
    {
        if (inspection.PreviousInspectionId == null)
        {
            return null;
        }

        var parentInspection = await inspectionsRepository.GetById(inspection.PreviousInspectionId.Value);

        while (parentInspection?.PreviousInspectionId != null)
        {
            parentInspection = await inspectionsRepository.GetById(parentInspection.PreviousInspectionId.Value);
        }

        return parentInspection;
    }

    private async Task<(List<GetInspectionByRootResponse>, Pagination)> FilterAndPaginateInspections(bool? grouped,
        List<Guid>? icdRoots, int? page, int? size, List<Inspection> inspections)
    {
        grouped ??= false;
        icdRoots ??= new List<Guid>();

        if (grouped.Value)
        {
            inspections = inspections.Where(i => i.PreviousInspectionId == null).ToList();
        }

        List<GetInspectionByRootResponse> response = new List<GetInspectionByRootResponse>();

        foreach (var inspection in inspections)
        {
            var diagnoses = await diagnosesRepository.GetAllByInspection(inspection.Id);
            var mainDiagnosis = diagnoses
                .FirstOrDefault(d => d.Type == DiagnosisType.Main);

            if (mainDiagnosis == null)
            {
                continue;
            }

            if (icdRoots != null && icdRoots.Count > 0 && !icdRoots.Contains(mainDiagnosis.Icd10Id))
            {
                var mainDiagnosisIcdRoot = await icd10Repository.GetRootByChildId(mainDiagnosis.Icd10Id);

                if (mainDiagnosisIcdRoot == null || !icdRoots.Contains(mainDiagnosisIcdRoot.Id))
                {
                    continue;
                }
            }

            var author = await doctorsRepository.GetById(inspection.DoctorId);
            var patient = await patientsRepository.GetById(inspection.PatientId);

            var child = await inspectionsRepository.GetByParentInspectionId(inspection.Id);

            bool hasNested = child != null;

            bool hasChain = !(!hasNested && inspection.PreviousInspectionId == null);

            Icd10Entity icd = await icd10Repository.GetById(mainDiagnosis.Icd10Id);

            GetDiagnosisResponse diagnosisResponse = new GetDiagnosisResponse(mainDiagnosis.Id,
                mainDiagnosis.CreateTime, icd.Code, icd.Name, mainDiagnosis.Description, mainDiagnosis.Type);

            GetInspectionByRootResponse inspectionResponse = new GetInspectionByRootResponse(inspection.Id,
                inspection.CreateTime, inspection.PreviousInspectionId, inspection.Date, inspection.Conclusion,
                author.Id, author.Name, patient.Id, patient.Name, diagnosisResponse, hasChain, hasNested);

            response.Add(inspectionResponse);
        }

        return paginationService.PaginateList(response, page, size);
    }

    private async Task ValidateIcdRoots(List<Guid> icdRoots)
    {
        var roots = await icd10Repository.GetRoots();

        foreach (var icdRoot in icdRoots)
        {
            if (roots.Find(r => r.Id == icdRoot) == null)
            {
                throw new IncorrectFieldException("icdRoots", $"ICD {icdRoot} не является корневым эллементом МКБ");
            }
        }
    }
}