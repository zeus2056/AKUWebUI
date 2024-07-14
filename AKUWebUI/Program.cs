using AKUWebUI;
using BusinessLayer.Abstract.EFCore;
using BusinessLayer.Concrete.EFCore;
using DataAccessLayer.Abstract.EFCore;
using DataAccessLayer.Concrete.EFCore;
using EntityLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Rotativa.AspNetCore;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WebContext>();
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<WebContext>().AddDefaultTokenProviders();

//Repository sections
builder.Services.AddScoped<IEFCoreBranchRepository, EFCoreBranchRepository>();
builder.Services.AddScoped<IEFCoreStudentRepository, EFCoreStudentRepository>();
builder.Services.AddScoped<IEFCoreStaffRepository, EFCoreStaffRepository>();
builder.Services.AddScoped<IEFCoreParentRepository, EFCoreParentRepository>();
builder.Services.AddScoped<IEFCoreAgeGroupRepository, EFCoreAgeGroupRepository>();
builder.Services.AddScoped<IEFCoreRateRepository, EFCoreRateRepository>();
builder.Services.AddScoped<IEFCoreRateStudentRepository, EFCoreRateStudentRepository>();
builder.Services.AddScoped<IEFCoreExamRepository, EFCoreExamRepository>();
builder.Services.AddScoped<IEFCoreBankRepository, EFCoreBankRepository>();
builder.Services.AddScoped<IEFCorePaymentTypeRepository, EFCorePaymentTypeRepository>();
builder.Services.AddScoped<IEFCoreDiscountRateRepository, EFCoreDiscountRateRepository>();
builder.Services.AddScoped<IEFCoreExamResultRepository, EFCoreExamResultRepository>();
builder.Services.AddScoped<IEFCorePaymentRepository, EFCorePaymentRepository>();
builder.Services.AddScoped<IEFCoreRatePaymentInfoRepository, EFCoreRatePaymentInfoRepository>();
builder.Services.AddScoped<IEFCoreHistoryPaymentRepository, EFCoreHistoryPaymentRepository>();
builder.Services.AddScoped<IEFCorePermissionRepository, EFCorePermissionRepository>();
builder.Services.AddScoped<IEFCoreStaffPermissionRepository, EFCoreStaffPermissionRepository>();
builder.Services.AddScoped<IEFCoreFrontPaymetRepository, EFCoreFrontPaymentRepository>();


//Service Sections
builder.Services.AddScoped<IEFCoreBranchService, EFCoreBranchManager>();
builder.Services.AddScoped<IEFCoreStudentService, EFCoreStudentManager>();
builder.Services.AddScoped<IEFCoreStaffService, EFCoreStaffManager>();
builder.Services.AddScoped<IEFCoreParentService, EFCoreParentManager>();
builder.Services.AddScoped<IEFCoreAgeGroupService, EFCoreAgeGroupManager>();
builder.Services.AddScoped<IEFCoreRateService, EFCoreRateManager>();
builder.Services.AddScoped<IEFCoreRateStudentService, EFCoreRateStudentManager>();
builder.Services.AddScoped<IEFCoreExamService, EFCoreExamManager>();
builder.Services.AddScoped<IEFCoreBankService, EFCoreBankManager>();
builder.Services.AddScoped<IEFCorePaymentTypeService, EFCorePaymentTypeManager>();
builder.Services.AddScoped<IEFCoreDiscountRateService, EFCoreDiscountRateManager>();
builder.Services.AddScoped<IEFCoreExamResultService, EFCoreExamResultManager>();
builder.Services.AddScoped<IEFCorePaymentService, EFCorePaymentManager>();
builder.Services.AddScoped<IEFCoreRatePaymentInfoService, EFCoreRatePaymentInfoManager>();
builder.Services.AddScoped<IEFCoreHistoryPaymentService, EFCoreHistoryPaymentManager>();
builder.Services.AddScoped<IEFCorePermissionService, EFCorePermissionManager>();
builder.Services.AddScoped<IEFCoreStaffPermissionService, EFCoreStaffPermissionManager>();
builder.Services.AddScoped<IEFCoreFrontPaymentService, EFCoreFrontPaymentManager>();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Login/Index";
    o.LogoutPath = "/Login/Logout";
    o.AccessDeniedPath = "/Login/AccessDenied";
    o.Cookie = new CookieBuilder()
    {
        HttpOnly = true
    };
});





var app = builder.Build();


RotativaConfiguration.Setup("Rotativa");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();




app.MapControllerRoute(
    name: "student-detail",
    pattern: "Students/Detail/{name?}",
    defaults: new
    {
        controller = "Students",
        action = "Details"
    });
app.MapControllerRoute(
    name: "student-edit",
    pattern: "Students/Edit/{slug?}",
    defaults: new
    {
        controller = "Students",
        action = "EditStudent"
    });
app.MapControllerRoute(
    name: "student-changestate",
    pattern: "Student/ChangeState/{slug?}",
    defaults: new
    {
        controller = "Students",
        action = "ChangeState"
    });
app.MapControllerRoute(
    name: "student-showrates",
    pattern: "Student/ShowRates/{slug?}",
    defaults: new
    {
        controller = "Students",
        action = "ShowRates"
    });
app.MapControllerRoute(
    name: "student-showexams",
    pattern: "Student/ShowExam/{slug?}",
    defaults: new
    {
        controller = "Students",
        action = "ShowExams"
    });
app.MapControllerRoute(
    name: "student-showhistorypayments",
    pattern: "Student/HistoryPayment/{slug?}",
    defaults: new
    {
        controller = "Students",
        action = "ShowHistoryPayment"
    });

app.MapControllerRoute(
    name: "staff-changePassword",
    pattern: "Staff/ChangePassword/{slug?}",
    defaults: new
    {
        controller = "Login",
        action = "ChangePassword"
    });
app.MapControllerRoute(
    name: "staff-details",
    pattern: "Staff/Details/{name?}",
    defaults: new
    {
        controller = "Staffs",
        action = "Details"
    }
    );
app.MapControllerRoute(
    name: "staff-edit",
    pattern: "Staff/Edit/{slug?}",
    defaults: new
    {
        controller = "Staffs",
        action = "EditStaff"
    }
    );
app.MapControllerRoute(
    name: "staff-addpermission",
    pattern: "Staff/AddPermission/{slug?}",
    defaults: new
    {
        controller = "Staffs",
        action = "AddPermission"
    }
    );
app.MapControllerRoute(
    name: "staff-emailedit",
    pattern: "Staff/EmailEdit/{slug?}",
    defaults: new
    {
        controller = "Staffs",
        action = "EditEmail"
    });
app.MapControllerRoute(
    name: "staff-phonedit",
    pattern: "Staff/PhoneEdit/{slug?}",
    defaults: new
    {
        controller = "Staffs",
        action = "EditPhone"
    });
app.MapControllerRoute(
    name: "parent-createparent",
    pattern: "Parent/CreateParent/{slug?}",
    defaults: new
    {
        controller = "Parents",
        action = "CreateParent"
    }
    );
app.MapControllerRoute(
    name: "parent-editparent",
    pattern: "Parent/EditParent/{slug?}",
    defaults: new
    {
        controller = "Parents",
        action = "EditParent"
    }
    );

app.MapControllerRoute(
    name: "parent-deleteparent",
    pattern: "Parent/DeleteParent/{slug?}",
    defaults: new
    {
        controller = "Parents",
        action = "DeleteParent"
    }
    );
app.MapControllerRoute(
    name: "rate-deleterate",
    pattern: "Rate/EditRate/{slug?}",
    defaults: new
    {
        controller = "Rates",
        action = "EditRate"
    }
    );
app.MapControllerRoute(
    name: "rate-deleterate",
    pattern: "Rate/DeleteRate/{slug?}",
    defaults: new
    {
        controller = "Rates",
        action = "DeleteRate"
    }
    );
app.MapControllerRoute(
    name: "rate-addratestudent",
    pattern: "Rate/RateStudents/AddStudent/{slug?}/{branchId?}",
    defaults: new
    {
        controller = "Rates",
        action = "AddRateStudent"
    }
    );

app.MapControllerRoute(
    name: "examtype-updateExamType",
    pattern: "ExamType/Edit/{slug?}",
    defaults: new
    {
        controller = "ExamTypes",
        action = "UpdateExamType"
    });
app.MapControllerRoute(
    name: "examtype-updateExamType",
    pattern: "ExamType/Delete/{slug?}",
    defaults: new
    {
        controller = "ExamTypes",
        action = "DeleteExamType"
    });

app.MapControllerRoute(
    name: "rate-showratestudents",
    pattern: "OpenRate/RateStudents/{slug}",
    defaults: new
    {
        controller = "OpenRates",
        action = "ShowStudents"
    }
    );

app.MapControllerRoute(
    name: "bank-bankedit",
    pattern: "Banks/Edit/{slug}",
    defaults: new
    {
        controller = "Banks",
        action = "UpdateBank"
    }
    );
app.MapControllerRoute(
    name: "bank-deletebank",
    pattern: "Banks/Delete/{slug}",
    defaults: new
    {
        controller = "Banks",
        action = "DeleteBank"
    }
    );
app.MapControllerRoute(
    name: "paymentType-update",
    pattern: "PaymentTypes/Edit/{slug}",
    defaults: new
    {
        controller = "PaymentTypes",
        action = "UpdatePaymentType"
    }
    );
app.MapControllerRoute(
    name: "paymentType-deletepaymenttype",
    pattern: "PaymentTypes/Delete/{slug}",
    defaults: new
    {
        controller = "PaymentTypes",
        action = "DeletePaymentType"
    }
    );
app.MapControllerRoute(
    name: "examresults-createexamresult",
    pattern: "ExamResult/Note/{slug?}/{id?}",
    defaults: new
    {
        controller = "ExamResults",
        action = "Index"
    }
    );
app.MapControllerRoute(
    name: "examresults-createexamresult",
    pattern: "ExamResult/Edit/{slug?}/{id?}",
    defaults: new
    {
        controller = "ExamResults",
        action = "UpdateNote"
    }
    );
app.MapControllerRoute(
    name: "openrates-removestudent",
    pattern: "OpenRates/State/Edit/{slug?}",
    defaults: new
    {
        controller = "OpenRates",
        action = "EditState"
    }
    );
app.MapControllerRoute(
    name: "openrates-removestudent",
    pattern: "OpenRates/Student/Remove/{slug?}",
    defaults: new
    {
        controller = "OpenRates",
        action = "RemoveStudent"
    }
    );
app.MapControllerRoute(
    name: "payment-payrate",
    pattern: "Student/Payment/{id?}/{studentId?}/{slug?}",
    defaults: new
    {
        controller = "Students",
        action = "PayRates"
    }
    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
