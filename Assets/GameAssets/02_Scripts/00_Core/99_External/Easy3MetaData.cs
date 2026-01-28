using Unity.Burst.CompilerServices;

public static class Easy3MetaData
{
    private static string UserName = "ChunBong";
    
    public static string UserDirectory = $"GameSave/{UserName}";
    public static string CoreFile = $"GameSave/{UserName}/CoreFile.es3";
    public static string ProgressFile = $"GameSave/{UserName}/ProgressFile.es3";


    // 추후 세이브 파일 선택 시 갱신할 세이브 파일 유저 이름
    public static void OnChooseSaveFile(string userName)
    {
        UserName = userName;
    }
}