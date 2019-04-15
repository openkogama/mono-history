using System;

[Flags]
public enum ShowUseOption : short
{
	Normal = 0,
	UsingGameCoins = 1,
	GameCoinsEnough = 2,
	GameCoinsInsufficient = 4,
	UsingLevels = 8,
	LevelEnough = 0x10,
	LevelInsufficient = 0x20,
	UsingStars = 0x40,
	StarsEnough = 0x80,
	StarsInsufficient = 0x100,
	UsingTeam = 0x200,
	TeamAllowed = 0x400,
	TeamRestricted = 0x800,
	UsingGameRank = 0x1000,
	GameRankEnough = 0x2000,
	GameRankInsufficient = 0x4000
}
