using Microsoft.AspNetCore.Components.QuickGrid;

namespace BNF.Timelapse.Web.Components.Info;

public partial class Info
{
    private class DriveListInfo
    {
        public required string Drive { get; set; }
        public required string FreeSpace { get; set; }
    }


    GridItemsProvider<DriveListInfo>? _driveListProvider;

    protected override Task OnInitializedAsync()
    {
        _driveListProvider = _ =>
        {
            var drives = DriveInfo.GetDrives();

            var ret = new List<DriveListInfo>();
            foreach (var drive in drives)
            {
                try
                {
                    ret.Add(new DriveListInfo { Drive = drive.Name, FreeSpace = GetSpaceText(drive.TotalFreeSpace) });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return ValueTask.FromResult(GridItemsProviderResult.From(
                items: ret,
                totalItemCount: ret.Count
            ));
        };
        return Task.CompletedTask;
    }

    private string GetSpaceText(long driveTotalFreeSpace)
    {
        // Don't @ me!
        long kilobyte = 1024;
        long megabyte = kilobyte * 1024;
        long gigabyte = megabyte * 1024;
        long terabyte = gigabyte * 1024;

        if (driveTotalFreeSpace > terabyte)
            return $"{(double)driveTotalFreeSpace / terabyte:0.000} TB";
        if (driveTotalFreeSpace > gigabyte)
            return $"{(double)driveTotalFreeSpace / gigabyte:0.000} GB";
        if (driveTotalFreeSpace > megabyte)
            return $"{(double)driveTotalFreeSpace / megabyte:0.000} MB";
        return $"{driveTotalFreeSpace} B";
    }
}