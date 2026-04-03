using Microsoft.AspNetCore.Components;
using AntDesign;
using StudentMgmt.GrpcServer;
using BlazorClient.Services.Student;
using BlazorClient.Services.Classroom;

namespace BlazorClient.Pages.Student
{
    public partial class Student
    {
        [Inject] public IStudentService StudentService { get; set; } = default!;
        [Inject] public IClassroomService ClassroomService { get; set; } = default!;
        [Inject] public IMessageService _message { get; set; } = default!;

        private IList<StudentResponse> Students { get; set; } = new List<StudentResponse>();
        private List<ClassroomResponse> _classrooms = new();
        private IEnumerable<StudentResponse> _selectedStudents = new List<StudentResponse>();

        private string SearchTerm { get; set; } = "";
        private int TotalCount { get; set; }
        private bool IsLoading { get; set; } = true;
        private int _pageIndex = 1;
        private int _pageSize = 10;

        private bool _modalVisible = false;
        private bool _confirmLoading = false;
        private bool _isEditMode = false;
        private string _editingId = string.Empty;
        private CreateStudentRequest _newStudent = new();

        // --- Bulk Update State ---
        private bool _changeClassModalVisible = false;
        private bool _updateClassEnabled, _updateAddressEnabled, _updateStatusEnabled;
        private string _targetClassId = "", _targetAddress = "", _targetStatus = "";

        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAll(LoadData(), LoadClassrooms());
        }

        public async Task LoadClassrooms() =>
            _classrooms = (await ClassroomService.GetAllClassroomsAsync(new EmptyRequest())).Items.ToList();

        public async Task LoadData()
        {
            IsLoading = true;
            try
            {
                var response = await StudentService.GetPagedStudentsAsync(SearchTerm, _pageIndex, _pageSize);
                Students = response.Items.ToList();
                TotalCount = response.TotalCount;
            }
            finally { IsLoading = false; StateHasChanged(); }
        }

        public async Task OnSearchClick() { _pageIndex = 1; await LoadData(); }
        public async Task ResetSearch() { SearchTerm = ""; _pageIndex = 1; await LoadData(); }
        public async Task HandlePageChange(PaginationEventArgs args) { _pageIndex = args.Page; _pageSize = args.PageSize; await LoadData(); }

        public void ShowModal() { _isEditMode = false; _newStudent = new CreateStudentRequest { DateOfBirth = DateTime.Now.ToString("yyyy-MM-dd") }; _modalVisible = true; }

        public void ShowEditModal(StudentResponse student)
        {
            _isEditMode = true; _editingId = student.Id;
            _newStudent = new CreateStudentRequest { Name = student.Name, Address = student.Address, DateOfBirth = student.BirthDate, ClassroomId = student.ClassroomId };
            _modalVisible = true;
        }

        public async Task HandleOk()
        {
            _confirmLoading = true;
            try
            {
                if (_isEditMode) await StudentService.UpdateStudentAsync(new UpdateStudentRequest { Id = _editingId, Name = _newStudent.Name, Address = _newStudent.Address, DateOfBirth = _newStudent.DateOfBirth, ClassroomId = _newStudent.ClassroomId });
                else await StudentService.CreateStudentAsync(_newStudent);
                _modalVisible = false; await LoadData(); await _message.Success("Thành công!");
            }
            finally { _confirmLoading = false; }
        }

        public async Task HandleDelete(string id) { await StudentService.DeleteStudentAsync(new DeleteStudentRequest { Id = id }); await LoadData(); await _message.Success("Đã xóa!"); }

        public void ShowBulkUpdateModal()
        {
            _updateClassEnabled = _updateAddressEnabled = _updateStatusEnabled = false;
            _targetClassId = _targetAddress = _targetStatus = "";
            _changeClassModalVisible = true;
        }

        public async Task HandleBulkUpdateOk()
        {
            if (!_updateClassEnabled && !_updateAddressEnabled && !_updateStatusEnabled)
            {
                await _message.Warning("Chọn ít nhất 1 trường!");
                return;
            }

            _confirmLoading = true;
            StateHasChanged();

            string loadingKey = "bulk_update_loading";
            _ = _message.Loading(new MessageConfig
            {
                Content = "Đang xử lý cập nhật hàng loạt...",
                Key = loadingKey,
                Duration = 0
            });

            try
            {
                var request = new BulkUpdateStudentsRequest();
                request.StudentIds.AddRange(_selectedStudents.Select(s => s.Id));

                if (_updateClassEnabled) { request.UpdateMask.Add("classroom_id"); request.ClassroomId = _targetClassId; }
                if (_updateAddressEnabled) { request.UpdateMask.Add("address"); request.Address = _targetAddress; }
                if (_updateStatusEnabled) { request.UpdateMask.Add("status"); request.Status = _targetStatus; }

                var result = await StudentService.BulkUpdateStudentsAsync(request);

                if (result.Success)
                {
                    _ = _message.Success(new MessageConfig { Content = result.Message, Key = loadingKey });

                    _changeClassModalVisible = false;
                    _selectedStudents = new List<StudentResponse>();
                    await LoadData();
                }
                else
                {
                    _ = _message.Error(new MessageConfig { Content = result.Message, Key = loadingKey });
                }
            }
            catch (Exception ex)
            {
                _ = _message.Error(new MessageConfig { Content = $"Lỗi: {ex.Message}", Key = loadingKey });
            }
            finally
            {
                _confirmLoading = false;
                StateHasChanged();
            }
        }
    }
}