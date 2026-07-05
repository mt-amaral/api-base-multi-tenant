using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Api.Configurations;

public static class ConfigApp
{
    public static class Cors
    {
        public static readonly string[] DevOrigins =
        [
            "http://localhost:4300"
        ];

        public static readonly string[] ProdOrigins =
        [
            "http://localhost:5002",
            "http://localhost:5002/"
        ];
    }

    /// ========== Entity ==========
    // Tamanho mínimo da senha
    public static int PasswordRequiredLength = 8;

    // Quantidade mínima de caracteres únicos na senha
    public static int PasswordRequiredUniqueChars = 1;


    // Se precisa confirmar conta antes de logar (email, etc)
    public static bool RequireConfirmedAccount = false;


    // Quais caracteres são permitidos no UserName
    public static string AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Quantas tentativas erradas antes de bloquear
    public static int LockoutMaxFailedAccessAttempts = 5;

    // Tempo que o usuário fica bloqueado
    public static TimeSpan LockoutDefaultTimeSpan = TimeSpan.FromMinutes(20);

    // Claim usada como identificador do usuário
    public static string UserIdClaimType = ClaimTypes.NameIdentifier;

    // Claim usada como nome do usuário
    public static string UserNameClaimType = ClaimTypes.Name;

    // Claim usada para roles/permissões
    public static string RoleClaimType = ClaimTypes.Role;

    // Chave identificadora do cookie de refresh token
    public static string RefreshTokenCookieName = "refreshToken";

    // Tempo de duração de token (minutos)
    public static int TokenCookieTime = 15;

    // Tempo de duração de token (minutos)
    public static int RefreshTokenCookieTime = 300;

    // ========== Rate Limiting ==========

    // Limites para clientes autenticados
    // Quantidade máxima de requisições permitidas por janela 
    public static int RateLimitPermitLimitAuthenticated = 200;

    // Tempo da janela de contagem  (segundos)
    public static int RateLimitWindowSeconds = 60;

    // Número de segmentos da janela
    public static int RateLimitSegmentsPerWindow = 6;

    // Quantidade máxima de requisições esperando na fila
    public static int RateLimitQueueLimitAuthenticated = 10;

    // Limites para clientes não autenticados
    public static int RateLimitPermitLimitAnonymous = 120;
    public static int RateLimitQueueLimitAnonymous = 5;

    // Se estourar o limite, processa a fila por ordem de chegada
    public static QueueProcessingOrder QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
}
