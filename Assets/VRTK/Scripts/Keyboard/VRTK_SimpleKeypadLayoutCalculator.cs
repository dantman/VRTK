﻿// Simple Keypad Layout Calculator|Keyboard|81041
namespace VRTK
{
    using UnityEngine;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// A keyboard layout calculator that lays out keys in evenly spaced rows on a single keypad
    /// </summary>
    /// <remarks>
    /// The simple keyboard layout calculator lays out keys row by row with such that
    /// keys are evenly sized within their row. If you use the same row count/weight in
    /// each row then the simple keyboard layout will lay out keys in an evenly spaced grid.
    /// 
    /// This layout renders as a single keypad with a single key area.
    /// 
    /// Special key weights can be applied to make some keys span multiple columns of the grid.
    /// </remarks>
    [VRTK_KeyLayoutCalculator(name = "Simple Keypad Layout", requireSource = true, helpList = new string[]
    {
        "Uses a KeyboardLayout",
        "Keys are evenly sized",
        "Single area keypad"
    })]
    public class VRTK_SimpleKeypadLayoutCalculator : VRTK_BaseKeyboardLayoutCalculator
    {
        [Tooltip("Size of the vertical space in between rows")]
        public float rowSpacing = 0f;
        [Tooltip("Side of the horizontal space in between keys")]
        public float keySpacing = 0f;

        protected override void CalculateKeyset(BKeyset keyset, Vector2[] containerSizes)
        {
            BKeyArea keypad = keyset.KeyArea("Keypad");
            // For keypads, ignore all containers but the first
            Vector2 containerSize = containerSizes[0];
            
            keypad.rect = new Rect(
                new Vector2(0, 0),
                new Vector2(containerSize.x, containerSize.y)
            );

            // TODO: Implicit space+done row
            BRow[] rows = keyset.Rows();
            float perRowSpacing = (rowSpacing * (float)(rows.Length - 1)) / (float)rows.Length;
            float rowHeight = containerSize.y / (float)rows.Length - perRowSpacing;
            float y = 0;
            foreach (BRow row in rows)
            {
                CalculateAreaRowKeys(
                    area: keypad,
                    row: row,
                    keys: row.Keys(),
                    y: y,
                    keyHeight: rowHeight
                );

                y += rowHeight + rowSpacing;
            }
        }

        protected virtual void CalculateAreaRowKeys(BKeyArea area, BRow row, BKey[] keys, float y, float keyHeight)
        {
            float rowWeight = BKey.RowWeight(keys);
            float perKeySpacing = (keySpacing * (float)(rowWeight - 1)) / (float)rowWeight;
            float keyWidth = (area.rect.size.x / (float)rowWeight) - perKeySpacing;

            float x = 0;
            foreach (BKey key in keys)
            {
                float keyWeight = (float)key.weight;
                float width = keyWidth * keyWeight + keySpacing * (keyWeight - 1f);

                RKey rKey = key.Create();
                rKey.rect = new Rect(
                    x,
                    y,
                    width,
                    keyHeight
                );

                area.AddKey(rKey);

                x += keyWidth + keySpacing;
            }
        }
    }
}
