var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Add this line to register HttpClient for dependency injection
builder.Services.AddHttpClient("CafeAPI", client => {
    client.BaseAddress = new Uri("https://localhost:7008/"); // Set the base address for the API
})
; 

// For sessions (used for the cart)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); //Used to serve static files (like images, CSS, JavaScript) from the wwwroot folder, this is necessary for the images to be displayed on the web application
app.UseSession(); // Enable session middleware to use sessions in the application, this should be placed before UseRouting and UseAuthorization to ensure that session data is available for those middlewares and the Razor Pages that require it

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
