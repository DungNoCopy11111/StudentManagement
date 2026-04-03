using Microsoft.AspNetCore.Components;
using ChartJs.Blazor;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Common;
using StudentMgmt.GrpcServer;
using BlazorClient.Services.Student;

namespace BlazorClient.Pages.Student
{
    public partial class Dashboard : ComponentBase
    {
        [Inject] public IStudentService StudentService { get; set; } = default!;

        protected GetDashboardResponse? _data;
        protected PieConfig? _pieConfig;
        protected BarConfig? _barConfig;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _data = await StudentService.GetDashboardAsync();
                if (_data != null)
                {
                    SetupCharts();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gọi Dashboard: {ex.Message}");
            }
        }

        private void SetupCharts()
        {
            if (_data == null) return;
            _pieConfig = new PieConfig { Options = new PieOptions { Responsive = true } };
            var pieDataset = new PieDataset<double>(_data.ByClass.Select(x => (double)x.Value))
            {
                BackgroundColor = new[] { "#1890ff", "#2fc25b", "#facc14", "#f04864", "#8543e0" }
            };

            foreach (var label in _data.ByClass.Select(x => x.Label))
            {
                _pieConfig.Data.Labels.Add(label);
            }
            _pieConfig.Data.Datasets.Add(pieDataset);

            _barConfig = new BarConfig { Options = new BarOptions { Responsive = true } };
            var barDataset = new BarDataset<double>(_data.ByStatus.Select(x => (double)x.Value))
            {
                Label = "Số lượng sinh viên",
                BackgroundColor = "#1890ff"
            };

            foreach (var label in _data.ByStatus.Select(x => x.Label))
            {
                _barConfig.Data.Labels.Add(label);
            }
            _barConfig.Data.Datasets.Add(barDataset);
        }
    }
}