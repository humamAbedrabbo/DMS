using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Services
{
    public static class LocalService
    {
        private static Dictionary<string, string> dic = new Dictionary<string, string> 
        { 
            ["Select File"] = "اختر ملف",
            ["Go To Document"] = "فتح صفحة الوثيقة",
            ["Meta field has hierarcial nature. It will be added to all childs in tree structure if it was added to a parent, but it will not be removed automatically"] = "تمتلك الحقول الإضافية خواص توريثية. حيث أنها تضاف تلقائيا على كل فروع الشجرة وذلك بمجر إضافتها على فرع الشجرة الرئيسي ولا تحذف من الفروع بشكل تلقائي",
            ["Meta fields are additional attributes that can be attached to document, folder or repository"] = "الحقول الإضافية هي عبارة عن مجموعة خواص يمكن إلحاقها بوثيقة أو فهرس أو مكتبة",
            ["Language"] = "اللغة",
            ["Select repositories for user"] = "اختر مكتبات المستخدم",
            ["User Repositories"] = "مكتبات المستخدم",
            ["Accounts"] = "حسابات المستخدمين",
            ["Managing user accounts"] = "إدارة حسابات المستخدمين",
            ["Document Details"] = "بيانات الوثيقة",
            ["Folders Tree"] = "شجرة الفهارس",
            ["Repository Details"] = "بيانات المكتبة",
            ["Folder Details"] = "بيانات الفهرس",
            ["Repositories are archive containers that can store documents on either a database or directory"] = "المكتبات هي عبارة عن مخزن لأرشفة الوثائق في قاعدة البيانات أو في مجلد محدد",
            ["Reset"] = "عملية جديدة",
            ["Path"] = "الفهرس",
            ["Parent Folder"] = "الفهرس الأب",
            ["Done"] = "تمت العملية",
            ["Home"] = "الصفحة الرئيسية",
            ["DMS"] = "الأرشيف",
            ["Document Management System"] = "نظام أرشفة الوثائق",
            ["Add"] = "إضافة",
            ["Select Repository"] = "اختر مكتبة",
            ["Public"] = "قراءة فقط",
            ["Archive"] = "مؤرشف",
            ["Admin"] = "مدير",
            ["Add User"] = "إضافة مستخدم",
            ["Email"] = "البريد الالكتروني",
            ["User Name"] = "اسم المستخدم",
            ["Users"] = "المستخدمين",
            ["Add Repository"] = "إضافة مكتبة",
            ["Repository Name"] = "اسم المكتبة",
            ["Repositories"] = "المكتبات",
            ["Edit Repository"] = "تعديل المكتبة",
            ["Storage"] = "نوع التخزين",
            ["List"] = "القائمة",
            ["Name"] = "الاسم",
            ["Title"] = "العنوان",
            ["Edit Meta Field"] = "تعديل الحقل",
            ["Group"] = "اسم المجموعة",
            ["Field Title"] = "عنوان الحقل",
            ["Loading"] = "جاري التحميل",
            ["Add Meta Field"] = "إضافة الحقل",
            ["Default Value"] = "القيمة الافتراضية",
            ["Field Type"] = "نوع الحقل",
            ["Save"] = "حفظ",
            ["Close"] = "إغلاق",
            ["Description"] = "شرح تفصيلي",
            ["Title"] = "العنوان",
            ["Folder Name"] = "اسم المجلد",
            ["Add Folder"] = "إضافة مجلد",
            ["Folder"] = "المجلد",
            ["Upload Document"] = "تحميل وثيقة",
            ["Folders"] = "المجلدات",
            ["Upload"] = "تحميل",
            ["Date"] = "التاريخ",
            ["User"] = "المستخدم",
            ["Operation"] = "العملية",
            ["History"] = "العمليات السابقة",
            ["Set Value"] = "حفظ القيمة",
            ["Field Value"] = "القيمة",
            ["Field Name"] = "اسم الحقل",
            ["Meta Fields"] = "الحقول الإضافية",
            ["bytes"] = "بايت",
            ["Operation On"] = "تاريخ التعديل",
            ["Operation By"] = "اسم منفذ التعديل",
            ["Last Operation"] = "آخر عملية تعديل",
            ["Size"] = "حجم الوثيقة",
            ["Content Type"] = "نوع المحتوى",
            ["Version"] = "رقم النسخة",
            ["Repository"] = "المكتبة",
            ["Check In"] = "تحميل النسخة الجديدة",
            ["Check Out"] = "تعديل النسخة الحالية",
            ["Download"] = "تحميل",
            ["by"] = "من قبل",
            ["Document"] = "وثيقة",
            ["created on"] = "تاريخ الإنشاء",
            ["last update on"] = "تاريخ آخر تعديل"
        };

        public static string Get(string key, string lang = "en")
        {
            if(dic != null)
            {
                if(dic.ContainsKey(key))
                {
                    return lang == "en" ? key : dic[key];
                }
            }

            return key;
        }
    }
}
