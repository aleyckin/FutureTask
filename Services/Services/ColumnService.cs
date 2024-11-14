using AutoMapper;
using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.ProjectDtos;
using Contracts.Dtos.UserDtos;
using Domain.Entities;
using Domain.Exceptions.AbstractExceptions;
using Domain.Exceptions.ColumnException;
using Domain.Exceptions.ColumnExceptions;
using Domain.Exceptions.ProjectExceptions;
using Domain.RepositoryInterfaces;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnRepository _columnRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public ColumnService(IColumnRepository columnRepository, IProjectRepository projectRepository, IUnitOfWork unitOfWork, IMapper mapper, IValidatorManager validatorManager)
        {
            _columnRepository = columnRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<ColumnDto> CreateAsync(Guid projectId, ColumnDtoForCreate columnDtoForCreate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(columnDtoForCreate, cancellationToken);

            var project = await _projectRepository.GetProjectByIdAsync(columnDtoForCreate.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(columnDtoForCreate.ProjectId);
            }
            if (projectId != columnDtoForCreate.ProjectId)
            {
                throw new ColumnCreatingErrorWithProjectDependency(projectId, columnDtoForCreate.ProjectId);
            }

            var column = _mapper.Map<Column>(columnDtoForCreate);

            _columnRepository.Insert(column);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ColumnDto>(column);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid projectId, Guid columnId, CancellationToken cancellationToken = default)
        {
            var column = await _columnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }
            if (projectId != column.ProjectId)
            {
                throw new ColumnCreatingErrorWithProjectDependency(projectId, column.ProjectId);
            }
            _columnRepository.Remove(column);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ColumnDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var columns = await _columnRepository.GetAllColumnsAsync(cancellationToken);
            return _mapper.Map<List<ColumnDto>>(columns);
        }

        public async Task<List<ColumnDto>> GetAllColumnsForProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var columns = await _columnRepository.GetAllColumnsForProjectAsync(projectId, cancellationToken);
            return _mapper.Map<List<ColumnDto>>(columns);
        }

        public async Task<ColumnDto> GetColumnById(Guid columnId, CancellationToken cancellationToken = default)
        {
            var column = await _columnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }
            return _mapper.Map<ColumnDto>(column);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid columnId, ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(columnDtoForUpdate, cancellationToken);

            var column = await _columnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }

            _mapper.Map(columnDtoForUpdate, column);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
