﻿using PoESkillTree.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Computation.Common.Builders.Equipment
{
    /// <summary>
    /// Represents a <see cref="Common.Model.Items.Enums.ItemSlot"/>.
    /// </summary>
    /// <remarks>
    /// Necessary to allow referencing and resolving item slots.
    /// </remarks>
    public interface IItemSlotBuilder : IResolvable<IItemSlotBuilder>
    {
        
    }
}