using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var _animals = new List<Animal>()
{
    new Animal {Id = 1, Name = "Szarik", Category = Category.Dog, Mass = 40.1, CoatColor = "brown"},
    new Animal {Id = 2, Name = "Filemon", Category = Category.Cat, Mass = 9.3, CoatColor = "black"},
    new Animal {Id = 3, Name = "Uszatek", Category = Category.Horse, Mass = 450.5, CoatColor = "grey"},
    new Animal {Id = 4, Name = "Stefan", Category = Category.Parrot, Mass = 0.1, CoatColor = "yellow"}
};

var _visits = new List<Visit>()
{
    new Visit {VisitDate = new DateTime(2022, 6, 1), TreatedAnimalId = 1, VisitDescription = "zlamany pazur", VisitPrice = 100},
    new Visit {VisitDate = new DateTime(2022, 6, 1), TreatedAnimalId = 2, VisitDescription = "odrobaczanie", VisitPrice = 150},
    new Visit {VisitDate = new DateTime(2022, 6, 3), TreatedAnimalId = 1, VisitDescription = "kontrola", VisitPrice = 50},
    new Visit {VisitDate = new DateTime(2022, 6, 4), TreatedAnimalId = 3, VisitDescription = "zabieg na lape", VisitPrice = 400}
};

Dictionary<Animal, List<Visit>> AnimalsAndVisits = new Dictionary<Animal, List<Visit>>();


foreach (Animal animal in _animals)
{
    var tempVisits = new List<Visit>();
    foreach (Visit visit in _visits)
    {
        if (animal.Id == visit.TreatedAnimalId)
        {
            tempVisits.Add(visit);
        }    
    }
    AnimalsAndVisits.Add(animal, tempVisits);
}

app.MapGet("/api/animals", () => Results.Ok(_animals))
    .WithName("GetAnimals")
    .WithOpenApi();

app.MapGet("/api/animals/{id:int}", (int id) =>
    {
        Animal animal = _animals.FirstOrDefault(a => a.Id == id);
        return animal == null ? Results.NotFound($"Animal with id {id} was not found") : Results.Ok(animal);
    })
    .WithName("GetAnimal")
    .WithOpenApi();

app.MapGet("/api/visits/{id:int}", (int id) =>
    {
        Animal animal = _animals.FirstOrDefault(a => a.Id == id);
        return animal == null
            ? Results.NotFound($"Animal with id {id} was not found")
            : Results.Ok(AnimalsAndVisits[animal]);
    })
    .WithName("GetVisitOfGivenAnimal")
    .WithOpenApi();

app.MapPost("/api/animals", (Animal animal) =>
    {
        _animals.Add(animal);
        return Results.StatusCode(StatusCodes.Status201Created);
    })
    .WithName("CreateAnimal")
    .WithOpenApi();

app.MapPost("/api/visits", (Visit visit) =>
    {
        _visits.Add(visit);
        Animal tempAnimal = _animals.FirstOrDefault(a => a.Id == visit.TreatedAnimalId);
        if (tempAnimal == null)
        {
            return Results.NotFound($"Animal with id {visit.TreatedAnimalId} was not found");
        }

        var tempList = AnimalsAndVisits[tempAnimal];
        
        tempList.Add(visit);

        AnimalsAndVisits[tempAnimal] = tempList;
        return Results.StatusCode(StatusCodes.Status201Created);
    })
    .WithName("CreateVisit")
    .WithOpenApi();

app.MapPut("/api/animals/{id:int}", (int id, Animal animal) =>
    {
        Animal animalToEdit = _animals.FirstOrDefault(a => a.Id == id);
        if (animalToEdit == null)
        {
            return Results.NotFound($"Animal with id {id} was not found");
        }

        _animals.Remove(animalToEdit);
        _animals.Add(animal);
        return Results.StatusCode(StatusCodes.Status200OK);
    })
    .WithName("UpdateAnimal")
    .WithOpenApi();

app.MapDelete("/api/animals/{id:int}", (int id) =>
    {
        Animal animalToDelete = _animals.FirstOrDefault(a => a.Id == id);
        if (animalToDelete == null)
        {
            return Results.NotFound($"Animal with id {id} was not found");
        }

        AnimalsAndVisits.Remove(animalToDelete);
        _visits.RemoveAll(v => v.TreatedAnimalId==id);

        _animals.Remove(animalToDelete);
        return Results.StatusCode(StatusCodes.Status200OK);
    })
    .WithName("DeleteAnimal")
    .WithOpenApi();

app.Run();
