export class Spell {
    public id: string;
    public name: string;
    public school: SpellSchool;
    public castAsRitual: boolean;
    public level: number;
    public time: string;
    public range: string;
    public vocalComponent: boolean;
    public somaticComponent: boolean;
    public materials: string;
    public duration: string;
    public text: string;
    public classLists: string[];
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