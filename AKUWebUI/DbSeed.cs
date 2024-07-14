using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AKUWebUI
{
    public  static class DbSeed
    {
        public  static  void Initialize(WebContext context)
        {

            if (!context.AgeGroups.Any())
            {
                 context.AgeGroups.AddRange(new AgeGroup() {  StartAge = 7, EndAge = 9, Name = "7-9 Yaş Grubu" }, new AgeGroup() {  StartAge = 9, EndAge = 11, Name = "9-11 Yaş Grubu" }, new AgeGroup() {  StartAge = 12, EndAge = 17, Name = "12-17 Yaş Grubu"}, new AgeGroup() {  StartAge = 18, EndAge = 60, Name = "Yetişkin" });
                 context.SaveChanges();
            }
            if (!context.Banks.Any())
            {
                 context.Banks.Add(new Bank() {  BankName = "Ziraat Bankası", Slug = "Ziraat-Bankası" });
                 context.SaveChanges();
            }
            if (!context.Branches.Any())
            {
                 context.Branches.AddRange(new Branch() {  BranchName = "Afyon" }, new Branch() { BranchName = "Eskişehir" });
                 context.SaveChanges();
            }
            if (!context.DiscountRates.Any())
            {
                 context.DiscountRates.AddRange(new DiscountRate() {  DiscontRateName = "0% İndirim", DiscountRates = 0 }, new DiscountRate() { DiscountRates = 10, DiscontRateName = "10% İndirim" });
                 context.SaveChanges();
            }
            if (!context.PaymentTypes.Any())
            {
                 context.PaymentTypes.AddRange(new PaymentType() {  PaymentTypeName = "nakit", Slug = "nakit" }, new PaymentType() { Slug = "Havale", PaymentTypeName = "Havale"}, new PaymentType() {  PaymentTypeName = "Kredi Kartı", Slug = "Kredi-Kartı" });
                 context.SaveChanges();
            }
            if (!context.Permissions.Any())
            {
                 context.Permissions.AddRange(new Permission() { Name = "1-5 Yıllık Çalışan", DayCount = 14,  YearCount = 1 }, new Permission() { YearCount = 6 ,DayCount = 20, Name = "6-14 Yıllık Çalışan" }, new Permission() { Name = "15 Yıl Üstü Çalışan", DayCount = 26, YearCount = 15 });
                 context.SaveChanges();
                }
            
            if (!context.Rates.Any())
            {
                List<Rate> rates = new List<Rate>();
                var ageGroups =  context.AgeGroups.AsNoTracking().ToList();
                var branches =  context.Branches.AsNoTracking().ToList();
                foreach (var branch in branches)
                {
                    foreach (var ageGroup in ageGroups)
                    {
                        if (ageGroup.Name.ToLower() != "yetişkin")
                        {
                            for (var i = 1; i <= 6; i++)
                            {
                                var name = Guid.NewGuid().ToString();
                                rates.Add(new Rate() { AgeGroupId = ageGroup.AgeGroupId, BranchId = branch.BranchId, Description = "Deneme", RateDate = 30, RatePrice = 3000, RateStartDate = DateTime.Now, RateState = true, Slug = i + ".Kur" + name, RateName = i + ".Kur" });
                            }
                        }
                        else
                            break;
                    }
                }

               
                string[] names = new string[9] { "Yazılım Dilleri 1","Yazılım Dilleri 2", "OOP Programlama","Veri Tabanı Yön. Sis.", "Full Stack Web Prog.", "Mobil Programlama","Chat Gpt ve Komut Uzmanlığı", "Yapay Zeka ve NLP Eğitimi","Veri Bilimi ve Veri Analizi"};
                foreach (var branch in branches)
                {
                    foreach (var name in names)
                    {
                        var _name = Guid.NewGuid().ToString();
                        var ageGroup =  context.AgeGroups.AsNoTracking().FirstOrDefault(a => a.Name == "yetişkin");
                        rates.Add(new Rate() { AgeGroupId = ageGroup.AgeGroupId, BranchId = branch.BranchId, Description = "Deneme", RateDate = 30, RateName = name, RatePrice = 3000, RateStartDate = DateTime.Now, RateState = true, Slug = name+_name });
                    }
                }

                context.Rates.AddRange(rates);
                context.SaveChanges();
            }

        }
    }
}
