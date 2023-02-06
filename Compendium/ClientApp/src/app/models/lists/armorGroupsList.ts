import {ArmorGroup} from "../monster";

export class ArmorGroups {
    public name: string;
    public value: ArmorGroup;
}

export class ArmorGroupsList {
    public static list: ArmorGroups[] = [
        { name: "Natural Armor", value: ArmorGroup.NaturalArmor },
        { name: "Light Armor", value: ArmorGroup.LightArmor },
        { name: "Medium Armor", value: ArmorGroup.MediumArmor },
        { name: "Heavy Armor", value: ArmorGroup.HeavyArmor }
    ]
}