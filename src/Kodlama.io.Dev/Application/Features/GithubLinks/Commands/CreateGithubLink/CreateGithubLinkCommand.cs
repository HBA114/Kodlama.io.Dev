﻿using Application.Features.GithubLinks.Dtos;
using Application.Features.GithubLinks.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Domain.Entities;
using MediatR;

namespace Application.Features.GithubLinks.Commands.CreateGithubLink;

public class CreateGithubLinkCommand : IRequest<CreatedGithubLinkDto>, ISecuredRequest
{
    public int UserId { get; set; }
    public string? Url { get; set; }

    public string[] Roles { get; } = new string[1] { "User" }; // ?? 

    public class CreateGithubLinkCommandHandler : IRequestHandler<CreateGithubLinkCommand, CreatedGithubLinkDto>
    {
        private readonly IGithubLinkRepository _githubLinkRepository;
        private readonly IMapper _mapper;
        private readonly GithubLinkBusinessRules _githubLinkBusinessRules;

        public CreateGithubLinkCommandHandler(IGithubLinkRepository githubLinkRepository, IMapper mapper, GithubLinkBusinessRules githubLinkBusinessRules)
        {
            _githubLinkRepository = githubLinkRepository;
            _mapper = mapper;
            _githubLinkBusinessRules = githubLinkBusinessRules;
        }

        public async Task<CreatedGithubLinkDto> Handle(CreateGithubLinkCommand request, CancellationToken cancellationToken)
        {
            await _githubLinkBusinessRules.CanNotAddSecondLinkToUser(request.UserId);

            GithubLink mappedUserLink = _mapper.Map<GithubLink>(request);
            GithubLink createdUserLink = await _githubLinkRepository.AddAsync(mappedUserLink);

            CreatedGithubLinkDto result = _mapper.Map<CreatedGithubLinkDto>(createdUserLink);
            return result;
        }
    }
}

