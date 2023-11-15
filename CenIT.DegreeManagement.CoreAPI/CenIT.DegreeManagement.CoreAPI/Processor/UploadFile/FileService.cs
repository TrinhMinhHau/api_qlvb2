using CenIT.DegreeManagement.CoreAPI.Controllers.Account;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Google.Api.Gax.ResourceNames;
using NPOI.HPSF;

namespace CenIT.DegreeManagement.CoreAPI.Processor.UploadFile
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _environment;
        private readonly ShareResource _localizer;

        public FileService(IWebHostEnvironment environment, ShareResource localizer)
        {
            _environment = environment;
            _localizer = localizer;
        }


        public Tuple<int, string> SaveFileImage(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create, access: FileAccess.ReadWrite);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();


                return new Tuple<int, string>(1, url);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, ex.Message);
            }
        }

        public Tuple<int, string> SaveImage(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create, access : FileAccess.ReadWrite);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();


                return new Tuple<int, string>(1, fileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, ex.Message);
            }
        }

        public Tuple<int, string> SaveFilePDFOrWorld(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".pdf", ".doc", ".docx" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();

                return new Tuple<int, string>(1, fileName);

            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, _localizer.GetUploadErrorMessage());
            }
        }

        public Tuple<int, string> SaveFile(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".pdf", ".doc", ".docx" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();

                return new Tuple<int, string>(1, url);

            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, _localizer.GetUploadErrorMessage());
            }
        }


        public int DeleteFile(string fileName, string pathFolder)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, pathFolder);
                var filePath = Path.Combine(path, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error deleting file: {ex.Message}";
                return 0;
            }
        }

        public Tuple<int, string> SaveFileExel(MemoryStream stream, string folderName, string fileName, string fileExtension)
        {
            throw new NotImplementedException();
        }
    }
}
