using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IChatbotService
    {
        Task<(bool Success, string? ErrorMessage, ChatResponseDto? Result)> SendMessageAsync(ChatRequestDto dto);
        Task<(bool Success, string? ErrorMessage, List<TinNhanDto>? Result)> GetLichSuAsync(int khachHangId);
    }
}