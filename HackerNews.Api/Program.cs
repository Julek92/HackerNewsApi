using HackerNews.Api.Cache;
using HackerNews.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(c =>
    c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/"));
builder.Services.AddScoped<IBestStoryRetriever, BestStoryRetriever>();
var cacheLifetimeInSeconds = builder.Configuration.GetValue<int>("CacheLifetimeSeconds");
var cacheLifetime = TimeSpan.FromSeconds(cacheLifetimeInSeconds);
builder.Services.AddSingleton<BestIdsThreadSafeCache>(_ => new BestIdsThreadSafeCache(cacheLifetime));
builder.Services.AddSingleton<StoriesThreadSafeCache>(_ => new StoriesThreadSafeCache(cacheLifetime));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();

namespace HackerNews.Api
{
    public partial class Program
    {
    }
}