using DoctorSlotAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//for allow Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:7132")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddHttpClient<ISlotService>();
builder.Services.Configure<SlotServiceSettings>(builder.Configuration.GetSection("SlotServiceSettings"));
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
