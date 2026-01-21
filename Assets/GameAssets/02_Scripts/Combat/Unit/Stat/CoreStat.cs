using DataEntity;
using DataEnum;

public class CoreStat
{
    private readonly EntityData _baseData;
    public CoreStat(EntityData baseData)
    {
        _baseData = baseData;
    }

    public string ID => _baseData.code;
    public SIDE Side => _baseData.Side;
    public string Name => _baseData.Name;
    public string AssetFileName => _baseData.Asset_File;
    public string[] SkillsID => _baseData.Skill_ID;
}