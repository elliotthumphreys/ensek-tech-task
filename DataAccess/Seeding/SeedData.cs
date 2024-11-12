using DataAccess.Repositories;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Seeding;

public static class SeedData
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        var accountRepo = serviceProvider.GetRequiredService<IDataAccessRepository<Account>>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        if (accountRepo.Get(x => true).Any())
            return; // already seeded

        accountRepo.Add(new(2344,"Tommy", "Test"));
        accountRepo.Add(new(2233,"Barry", "Test"));
        accountRepo.Add(new(8766,"Sally", "Test"));
        accountRepo.Add(new(2345,"Jerry", "Test"));
        accountRepo.Add(new(2346,"Ollie", "Test"));
        accountRepo.Add(new(2347,"Tara", "Test"));
        accountRepo.Add(new(2348,"Tammy", "Test"));
        accountRepo.Add(new(2349,"Simon", "Test"));
        accountRepo.Add(new(2350,"Colin", "Test"));
        accountRepo.Add(new(2351,"Gladys", "Test"));
        accountRepo.Add(new(2352,"Greg", "Test"));
        accountRepo.Add(new(2353,"Tony", "Test"));
        accountRepo.Add(new(2355,"Arthur", "Test"));
        accountRepo.Add(new(2356,"Craig", "Test"));
        accountRepo.Add(new(6776,"Laura", "Test"));
        accountRepo.Add(new(4534,"JOSH", "TEST"));
        accountRepo.Add(new(1234,"Freya", "Test"));
        accountRepo.Add(new(1239,"Noddy", "Test"));
        accountRepo.Add(new(1240,"Archie", "Test"));
        accountRepo.Add(new(1241,"Lara", "Test"));
        accountRepo.Add(new(1242,"Tim", "Test"));
        accountRepo.Add(new(1243,"Graham", "Test"));
        accountRepo.Add(new(1244,"Tony", "Test"));
        accountRepo.Add(new(1245,"Neville", "Test"));
        accountRepo.Add(new(1246,"Jo", "Test"));
        accountRepo.Add(new(1247,"Jim", "Test"));
        accountRepo.Add(new(1248,"Pam", "Test"));

        unitOfWork.SaveChanges();
    }
}
