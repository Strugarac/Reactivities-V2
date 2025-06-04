// using System;
// using Application.Core;
// using Application.Interfaces;
// using Domain;
// using MediatR;
// using Microsoft.EntityFrameworkCore.Update;

// namespace Application.Profiles.Commands;

// public class ConfirmEmail
// {
//     public class Command : IRequest<Result<Unit>>
//     {
//         public required User User { get; set; }
//     }

//     public class Handler(IEmailService emailService, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
//     {
//         public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
//         {
//             var code = await signInManager.UserManager.GenerateEmailConfirmationTokenAsync(request.User);
//             code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

//             var subject = "Confirm Email";
//             var body = $@"
//                 <p>Hi {request.User.Displayname} </p>
//                 <p> Please click on the link bellow and confirm your email </p>
//                 <a href='{config["ClientAppUrl"]}/confirm-email?userId={user.Id}&code={code}'>CONFIRM</a>
//             ";
//             await emailService.SendConfirmationLinkAsync(registerDto.Email, subject, body);
            
//         }
//     }
// }
