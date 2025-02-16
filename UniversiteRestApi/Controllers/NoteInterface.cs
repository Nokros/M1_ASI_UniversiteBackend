using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NotesUseCases.Create;
using UniversiteDomain.UseCases.NotesUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public NoteController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        [HttpGet("generate/{numeroUe}")]
        public async Task<IActionResult> GenerateCsvAsync(string numeroUe)
        {
            if (!CheckSecurity(out string role, out string email, out IUniversiteUser user))
                return Unauthorized();
            
            var useCase = new CreateNotesUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            var csvStream = await useCase.ExecuteAsync(numeroUe);
            return File(csvStream, "text/csv", $"notes_{numeroUe}.csv");
        }

        [HttpPut("upload/{numeroUe}")]
        public async Task<IActionResult> UploadCsvAsync(string numeroUe, IFormFile csvFile)
        {
            if (!CheckSecurity(out string role, out string email, out IUniversiteUser user))
                return Unauthorized();
            
            var useCase = new CreateNotesFromCsvUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            using var memoryStream = new MemoryStream();
            await csvFile.CopyToAsync(memoryStream);
            byte[] fileContent = memoryStream.ToArray();

            try
            {
                await useCase.ExecuteAsync(numeroUe, fileContent);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UploadError", ex.Message);
                return ValidationProblem();
            }
            
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<List<NoteDto>>> GetAllNotesAsync()
        {
            if (!CheckSecurity(out string role, out string email, out IUniversiteUser user))
                return Unauthorized();
            
            var useCase = new GetToutesLesNotesUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role)) return Unauthorized();
            
            try
            {
                var notes = await useCase.ExecuteAsync();
                return NoteDto.ToDtos(notes);
            }
            catch (Exception)
            {
                return ValidationProblem();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> GetNoteByIdAsync(long id)
        {
            if (!CheckSecurity(out string role, out string email, out IUniversiteUser user))
                return Unauthorized();
            
            var useCase = new GetNoteByIdUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role, user, id)) return Unauthorized();
            
            try
            {
                var note = await useCase.ExecuteAsync(id);
                if (note == null) return NotFound();
                return new NoteDto().ToDto(note);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(ex), ex.Message);
                return ValidationProblem();
            }
        }

        [HttpGet("complet/{id}")]
        public async Task<ActionResult<NoteCompletDto>> GetCompleteNoteByIdAsync(long id)
        {
            if (!CheckSecurity(out string role, out string email, out IUniversiteUser user))
                return Unauthorized();
            
            var useCase = new GetNoteCompletUseCase(_repositoryFactory);
            if (!useCase.IsAuthorized(role, user, id)) return Unauthorized();
            
            try
            {
                var note = await useCase.ExecuteAsync(id);
                if (note == null) return NotFound();
                return new NoteCompletDto().ToDto(note);
            }
            catch (Exception)
            {
                return ValidationProblem();
            }
        }

        private bool CheckSecurity(out string role, out string email, out IUniversiteUser user)
        {
            role = string.Empty;
            email = string.Empty;
            user = null;
            
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            if (claimsPrincipal.Identity?.IsAuthenticated != true) return false;
            
            email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            role = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role)) return false;
            
            user = new FindUniversiteUserByEmailUseCase(_repositoryFactory).ExecuteAsync(email).Result;
            return user != null;
        }
    }
}