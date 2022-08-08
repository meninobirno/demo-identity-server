using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//adiciona a autentica��o nos servi�os e configura os esquemas padr�o (oidc e Cookies)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "oidc";
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
//configura o esquema Cookies
.AddCookie("Cookies", options => 
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Domain ="localhost";
    options.Cookie.Name = "demo-auth"; //nome do cookie usado pelo asp net core para manter a autentica��o
    options.Cookie.HttpOnly = false;
})
//configura o esquema oidc
.AddOpenIdConnect("oidc", options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = ""; //url do microsservi�o de autentica��o
    options.ClientId = "demo_ids"; //client que realizar� a autentica��o
    options.ResponseType = "code id_token"; //tipo de resposta do microsservi�o de autentica��o
    options.CallbackPath = "/signin-oidc"; //url de callback que autenticar� o usu�rio na aplica��o

    //adicionando os escopos necess�rios do client
    options.Scope.Add("openid");
    options.Scope.Add("profile");

    //validacao do token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType ="name",
        RoleClaimType = "value"
    };

});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//adicionando o middleware de autentica��o na aplica��o
app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
