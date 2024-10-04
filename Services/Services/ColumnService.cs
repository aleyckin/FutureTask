using AutoMapper;
using Contracts.Dtos.ColumnDtos;
using Contracts.Dtos.ProjectDtos;
using Domain.Entities;
using Domain.Exceptions.ColumnException;
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

        public ColumnService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<ColumnDto> CreateAsync(ColumnDtoForCreate columnDtoForCreate, CancellationToken cancellationToken = default)
        {
            var column = _mapper.Map<Column>(columnDtoForCreate);
            _repositoryManager.ColumnRepository.Insert(column);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ColumnDto>(column);
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid columnId, CancellationToken cancellationToken = default)
        {
            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }
            _repositoryManager.ColumnRepository.Remove(column);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ColumnDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var columns = await _repositoryManager.ColumnRepository.GetAllColumnsAsync(cancellationToken);
            return _mapper.Map<List<ColumnDto>>(columns);
        }

        public async Task<ColumnDto> GetColumnById(Guid columnId, CancellationToken cancellationToken = default)
        {
            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            return _mapper.Map<ColumnDto>(column);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Guid columnId, ColumnDtoForUpdate columnDtoForUpdate, CancellationToken cancellationToken = default)
        {
            var column = await _repositoryManager.ColumnRepository.GetColumnByIdAsync(columnId, cancellationToken);
            if (column == null)
            {
                throw new ColumnNotFoundException(columnId);
            }
            if (columnDtoForUpdate.Title != null)
            {
                column.Title = columnDtoForUpdate.Title;
            }
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
