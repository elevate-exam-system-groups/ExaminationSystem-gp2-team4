using Examination_System.Common.Models;
using Examination_System.Common.Repositories;
using Examination_System.Features.Diplomas.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Features.Diplomas.Queries
{
    public record GetActiveDiplomasQuery : IRequest<IQueryable<Diploma>>;
    public class GetActiveDiplomasQueryHandler : IRequestHandler<GetActiveDiplomasQuery, IQueryable<Diploma>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveDiplomasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IQueryable<Diploma>> Handle(GetActiveDiplomasQuery request, CancellationToken cancellationToken)
        {
            return _unitOfWork.Repository<Diploma>().GetAll().Where(d => d.IsActive);
        }
    }
}

      
  
               
        
    

