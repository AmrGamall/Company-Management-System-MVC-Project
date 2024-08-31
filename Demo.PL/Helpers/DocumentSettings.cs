using Microsoft.AspNetCore.Http;
using System;
using System.IO;
namespace Demo.PL.Helpers
{
    public class DocumentSettings
    {
        //Upload

        public static string UploadFile(IFormFile file, string FolderName)
        {
            // 1.Get located Folder path
            string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);

            //2.Get File Name and make it unique
            string FileName = $"{Guid.NewGuid()}{file.FileName}";

            //3.Get File Path [Folderpath , File Name]

            string FilePath = Path.Combine(FolderPath, FileName);

            //4.Save File as streams
            using var Fs = new FileStream(FilePath, FileMode.Create);
            file.CopyTo(Fs);

            //5.Return File Name
            return FileName;

        }


        //Delete

        public static void DeleteFile(string FolderName, string FileName) 
        { 
            //1.Get File Path

            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName, FileName);

            //2.check if file exists or not

            if(File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

    }
}
