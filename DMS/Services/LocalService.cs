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
