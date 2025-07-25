using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using StackExchange.Redis;
using StreamingAPI.Middlewares;
using StreamingApplication;
using StreamingInfrastructure;
using StreamingInfrastructure.Messaging.Consumers;
using StreamingShared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DI for each layer
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddShared();

// Add Masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailSendingConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("horse.lmq.cloudamqp.com", 5672, "pvuwlxfg", h =>
        {
            h.Username("pvuwlxfg");
            h.Password("hWdHwUAhT8CTwILoVGjq1RSQU-F9YSrp");
        });

        cfg.ReceiveEndpoint("email-sending-queue", e =>
        {
            e.ConfigureConsumer<EmailSendingConsumer>(context);
        });
    });
});

// Add Authentication JWT Bearer
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(option =>
    {
        option.SaveToken = true;
        option.RequireHttpsMetadata = false;
        option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

// Add  JWT Bearer Token to swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Thêm cấu hình cho JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token theo format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Setup Serilog
var columnWriters = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter() },
    { "level", new LevelColumnWriter() },
    { "time_stamp", new TimestampColumnWriter() },
    { "exception", new ExceptionColumnWriter() },
    { "log_event", new LogEventSerializedColumnWriter() },
    //{ "user_name", new SinglePropertyColumnWriter("UserName", propertyName: "user_name") }
};

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("StreamingDbConnection"),
        tableName: "systemlogs",
        columnOptions: columnWriters,
        needAutoCreateTable: true // auto-create the table if it doesn't exist
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Add memory cache
builder.Services.AddMemoryCache();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RateLimitMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
