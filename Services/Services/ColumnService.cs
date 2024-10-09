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
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public ColumnService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<ColumnDto> CreateAsync(Guid projectId, ColumnDtoForCreate columnDtoForCreate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(columnDtoForCreate, cancellationToken);

            var project = await _repositoryManager.ProjectRepository.GetProjectByIdAsync(columnDtoForCreate.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new ProjectNotFoundException(columnDtoForCreate.ProjectId);
            }
            if (projectId != columnDtoForCreate.ProjectId)
            {
                throw new ColumnCreatingErrorWithProjectDependency(projectId, columnDtoForCreate.ProjectId);
            }

            var column = _mapper.Map<Column>(columnDtoForCreate);

            _repositoryManager.ColumnRepository.Insert(column);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ColumnDto>(column);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid projectId, Guid columnId, CancellationToken cancellationToken = default)
        {
            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }
            if (projectId != column.ProjectId)
            {
                throw new ColumnCreatingErrorWithProjectDependency(projectId, column.ProjectId);
            }
            _repositoryManager.ColumnRepository.Remove(column);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ColumnDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var columns = await _repositoryManager.ColumnRepository.GetAllColumnsAsync(cancellationToken);
            return _mapper.Map<List<ColumnDto>>(columns);
        }

        public async Task<List<ColumnDto>> GetAllColumnsForProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            var columns = await _repositoryManager.ColumnRepository.GetAllColumnsForProjectAsync(projectId, cancellationToken);
            return _mapper.Map<List<ColumnDto>>(columns);
        }

        public async Task<ColumnDto> GetColumnById(Guid columnId, CancellationToken cancellationToken = default)
        {
            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }
            return _mapper.Map<ColumnDto>(column);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid columnId, ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken = default)
        {
            await _validatorManager.ValidateAsync(columnDtoForUpdate, cancellationToken);

            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }

            _mapper.Map(columnDtoForUpdate, column);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
