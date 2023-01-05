using Application.Features.OperationClaims.Dtos;
using Application.Features.OperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Security.Entities;
using MediatR;

namespace Application.Features.OperationClaims.Commands.DeleteOperationClaim;
public class DeleteOperationClaimCommand : IRequest<GetByIdOperationClaimDto>, ISecuredRequest
{
    public int Id { get; set; }
    public string[] Roles { get; } = { "Admin" };

    public class DeleteOperationClaimCommandHandler : IRequestHandler<DeleteOperationClaimCommand, GetByIdOperationClaimDto>
    {
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IMapper _mapper;
        private readonly OperationClaimBusinessRules _operationClaimBusinessRules;

        public DeleteOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository, IMapper mapper, OperationClaimBusinessRules operationClaimBusinessRules)
        {
            _operationClaimRepository = operationClaimRepository;
            _mapper = mapper;
            _operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<GetByIdOperationClaimDto> Handle(DeleteOperationClaimCommand request, CancellationToken cancellationToken)
        {
            OperationClaim? operationClaim = await _operationClaimRepository.GetAsync(x => x.Id == request.Id);
            _operationClaimBusinessRules.OperationClaimShouldExistWhenRequested(operationClaim);
            await _operationClaimRepository.DeleteAsync(operationClaim);
            GetByIdOperationClaimDto mappedGetByIdOperationClaimDto = _mapper.Map<GetByIdOperationClaimDto>(operationClaim);

            return mappedGetByIdOperationClaimDto;
        }
    }
}