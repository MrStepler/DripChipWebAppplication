using DripChipWebApplication.Server.Data;
using DripChipWebApplication.Server.Models;
using DripChipWebApplication.Server.Models.ResponseModels.AnimalType;
using DripChipWebApplication.Server.Services.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DripChipWebApplication.Server.Services
{
    public class AnimalTypesService : IAnimalTypesService
    {
        IDbContextFactory<APIDbContext> contextFactory;
        public AnimalTypesService(IDbContextFactory<APIDbContext> contextFactory) 
        {
            this.contextFactory = contextFactory;
        }

        public AnimalType AddType(string type)
        {
            using var dbContext = contextFactory.CreateDbContext();
            AnimalType createdType = new AnimalType();
            createdType.Type = type;
            dbContext.AnimalTypes.Add(createdType);
            dbContext.SaveChanges();
            return createdType;
        }

        public AnimalType EditType(long id, string type)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var editableType = dbContext.AnimalTypes.Find(id);
            editableType.Type = type;
            dbContext.SaveChanges();
            return editableType;
        }

        public AnimalType? GetType(long id)
        {
            using var dbContext = contextFactory.CreateDbContext();
            return dbContext.AnimalTypes.FirstOrDefault(t => t.Id == id);
        }
        public long[] GetTypesByAnimalId(long animalId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            return dbContext.AnimalTypes.Where(t => t.AnimalId == animalId).Select(t => t.Id).ToArray();
        }
        public List<AnimalType>? GetListTypesOfAnimal(long animalId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            if (!dbContext.AnimalTypes.Any(t => t.AnimalId == animalId))
            {
                return null;
            }
            return dbContext.AnimalTypes.Where(t => t.AnimalId == animalId).ToList();
        }
        public AnimalType? GetType(string type)
        {
            using var dbContext = contextFactory.CreateDbContext();
            return dbContext.AnimalTypes.FirstOrDefault(t => t.Type == type);
        }
        public Animal AddTypeToAnimal(long animalId, long typeId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var addableType = dbContext.AnimalTypes.Find(typeId);
            addableType.Animal = dbContext.Animals.Find(animalId);
            dbContext.SaveChanges();
            return dbContext.Animals.Find(animalId);
        }
        public Animal EditTypeOfAnimal(long animalId, SwitchableTypes switchableTypes)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var editableType = dbContext.AnimalTypes.Find(switchableTypes.OldTypeId);
            editableType.Animal = null;
            editableType.AnimalId = null;
            dbContext.SaveChanges();
            editableType = dbContext.AnimalTypes.Find(switchableTypes.NewTypeId);
            editableType.Animal = dbContext.Animals.Find(animalId);
            dbContext.SaveChanges();
            return dbContext.Animals.Find(animalId);
        }
        public Animal DeleteTypeOfAnimal(long animalId, long typeId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var deletableType = dbContext.AnimalTypes.First(x =>x.Id == typeId && x.AnimalId == animalId);
            deletableType.Animal = null;
            deletableType.AnimalId = null;
            dbContext.SaveChanges();
            return dbContext.Animals.First(x=>x.Id == animalId);
        }

        public void DeleteType(long typeId)
        {
            using var dbContext = contextFactory.CreateDbContext();
            var deletableType = dbContext.AnimalTypes.Find(typeId);
            dbContext.AnimalTypes.Remove(deletableType);
            dbContext.SaveChanges();
        }
    }
}
