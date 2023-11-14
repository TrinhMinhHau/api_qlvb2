namespace CenIT.DegreeManagement.CoreAPI.Processor.UploadFile
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile imageFile, string folderName);
        public Tuple<int, string> SaveFileImage(IFormFile imageFile, string folderName);

        public Tuple<int, string> SaveFilePDFOrWorld(IFormFile imageFile, string folderName);
        public Tuple<int, string> SaveFile(IFormFile imageFile, string folderName);
        public Tuple<int, string> SaveFileExel(MemoryStream stream, string folderName, string fileName, string fileExtension);

        public bool DeleteImage(string imageFileName);
    }
}
