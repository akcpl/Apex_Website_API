    using Apex_Website_API.Exceptions;
    using Apex_Website_API.Logging;
    using Apex_Website_API.Middleware;    
    using Apex_Website_API.Repositories.Implementations;
    using Apex_Website_API.Repositories.Interfaces;
    using Apex_Website_API.Services.Implementations;
    using Apex_Website_API.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.IdentityModel.Tokens;
    using Serilog;
    using System.Text;


    //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()).CreateLogger();
    var logRootPath = Path.Combine(AppContext.BaseDirectory,"Logs");
    //Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().WriteTo.Sink(new DailyLogFileSink(logRootPath)).CreateLogger();
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()).Enrich.FromLogContext().WriteTo.Console().WriteTo.Sink(new DailyLogFileSink(logRootPath)).CreateLogger();

    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog();

    builder.Services.AddControllers();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    // Repository
    builder.Services.AddScoped<IContactRepository, ContactRepository>();
    builder.Services.AddScoped<ICareerRepository, CareerRepository>();

    // Service
    builder.Services.AddScoped<IContactService, ContactService>();
    builder.Services.AddScoped<ICareerService, CareerService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    // AWS S3
    builder.Services.AddScoped<IS3Service, S3Service>();

    // JWT Authentication
    var jwtKey = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrWhiteSpace(jwtKey))
    {
        throw new InvalidOperationException("JWT Key is not configured.");
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT VALIDATION FAILED | Path: {Path} | Reason: {Reason}", context.HttpContext.Request.Path, context.Exception.Message);
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("JWT VALIDATION SUCCESS");

                return Task.CompletedTask;
            },

            OnChallenge = async context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT CHALLENGE | Path: {Path}", context.HttpContext.Request.Path);
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    message = "Unauthorized. Please provide a valid JWT token.",
                    traceId = context.HttpContext.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        };

    });

    // Authorization
    builder.Services.AddAuthorization();

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandler = async context =>
        {
            var exceptionHandler =context.RequestServices.GetRequiredService<GlobalExceptionHandler>();
            await exceptionHandler.TryHandleAsync(context,context.Features.Get<IExceptionHandlerFeature>()!.Error,context.RequestAborted);
        }
    });

    app.UseMiddleware<RequestLoggingMiddleware>();
    //app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();