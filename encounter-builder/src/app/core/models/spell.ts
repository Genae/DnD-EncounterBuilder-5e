export class Spell {
    public Name: string;
    public School: SpellSchool;
    public CastAsRitual: boolean;
    public Level: number;
    public Time: string;
    public Range: string;
    public VocalComponent: boolean;
    public SomaticComponent: boolean;
    public Materials: string;
    public Duration: string;
    public Text: string;
    public ClassLists: string[];
}

export enum SpellSchool
{
    Abjuration,
    Conjuration,
    Divination,
    Enchantment,
    Evocation,
    Illusion,
    Necromancy,
    Transmutation
}