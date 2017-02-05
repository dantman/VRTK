// Base Keyboard Layout Calculator|Keyboard|81040
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
    using VRTK.Keyboard;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using RKeyLayout = VRTK_RenderableKeyLayout;
    using RKeyset = VRTK_RenderableKeyLayout.Keyset;
    using RKeyArea = VRTK_RenderableKeyLayout.KeyArea;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// The Base Keyboard Layout Calculator is an abstract class that all keyboard layout calculators inherit
    /// </summary>
    /// <remarks>
    /// A keyboard layout calculator takes a `VRTK_KeyboardLayout` and generates a `VRTK_RenderableKeyLayout`
    /// with all key locations calculated.
    /// 
    /// How keys are laid out is up to the keyboard layout calculator, different styles of keyboard layouts may
    /// be implemented by different keyboard layout calculators.
    /// 
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_BaseKeyboardLayoutCalculator : VRTK_BaseKeyLayoutCalculator
    {
        [Tooltip("Keyboard layout to render")]
        public KeyboardLayout keyboardLayout;

        /// <summary>
        /// A high-level builder for renderable keyboard layouts
        /// </summary>
        protected class RKeyboardBuilder
        {
            private IKeyLayout<IRowKeyset<IRow<IKey>, IKey>> keyLayout;
            private RKeyLayout rKeyboard;

            public RKeyboardBuilder(IKeyLayout<IRowKeyset<IRow<IKey>, IKey>> keyLayout)
            {
                this.keyLayout = keyLayout;
                this.rKeyboard = new RKeyLayout();
            }

            /// <summary>
            /// Iterate over keysets automatically building the metadata for renderable keysets from keyboard layout keysets
            /// </summary>
            /// <returns>An iterator over keyset builders</returns>
            public IEnumerable<BKeyset> Keysets()
            {
                IRowKeyset<IRow<IKey>, IKey>[] keysets = keyLayout.GetKeysets();
                rKeyboard.keysets = new RKeyset[keysets.Length];
                for (int s = 0; s < keysets.Length; s++)
                {
                    IRowKeyset<IRow<IKey>, IKey> keyset = keysets[s];
                    RKeyset rKeyset = new RKeyset();
                    rKeyset.name = keyset.GetName();

                    BKeyset bKeyset = new BKeyset(s, keyset, rKeyset);
                    yield return bKeyset;
                    bKeyset.Commit();

                    rKeyboard.keysets[s] = rKeyset;
                }
            }

            /// <summary>
            /// Return a renderable keyboard layout once building is finalized
            /// </summary>
            /// <returns>The RenderableKeyLayout being built</returns>
            public RKeyLayout Commit()
            {
                return rKeyboard;
            }
        }

        /// <summary>
        /// A high-level builder for renderable keysets
        /// </summary>
        protected class BKeyset
        {
            /// <summary>
            /// The keyset index
            /// </summary>
            public readonly int index;
            /// <summary>
            /// The source Keyset
            /// </summary>
            public readonly IRowKeyset<IRow<IKey>, IKey> raw;
            /// <summary>
            /// The RenderableKeyLayout.Keyset being built
            /// </summary>
            public readonly RKeyset renderable;

            private List<RKeyArea> rAreas = new List<RKeyArea>();
            private List<BKeyArea> bAreas = new List<BKeyArea>();

            public BKeyset(int index, IRowKeyset<IRow<IKey>, IKey> raw, RKeyset renderable)
            {
                this.index = index;
                this.raw = raw;
                this.renderable = renderable;
            }

            /// <summary>
            /// Create a RenderableKeyLayout.KeyArea in this keyset
            /// </summary>
            /// <param name="name">Name to use for this key area</param>
            /// <returns>High-level builder for this key area</returns>
            public BKeyArea KeyArea(string name)
            {
                RKeyArea rKeyArea = new RKeyArea();
                rKeyArea.name = name;
                rAreas.Add(rKeyArea);

                BKeyArea bKeyArea = new BKeyArea(rKeyArea);
                bAreas.Add(bKeyArea);

                return bKeyArea;
            }

            /// <summary>
            /// Return an array of high-level builders for the rows in this keyset
            /// </summary>
            /// <returns>High-level row builders</returns>
            public BRow[] Rows()
            {
                IRow<IKey>[] rawRows = raw.GetRows();
                BRow[] rows = new BRow[rawRows.Length];
                for (int r = 0; r < rawRows.Length; r++)
                {
                    rows[r] = new BRow(r, rawRows[r]);
                }
                return rows;
            }

            /// <summary>
            /// Commit all key areas to their renderable key layout objects
            /// </summary>
            /// <remarks>
            /// This method is implicitly called in `CalculateKeyLayout` if you override the `CalculateKeyset`.
            /// </remarks>
            public void Commit()
            {
                foreach (BKeyArea bArea in bAreas)
                {
                    bArea.Commit();
                }

                this.renderable.areas = rAreas.ToArray();
            }
        }

        /// <summary>
        /// A high-level builder for renderable key areas
        /// </summary>
        protected class BKeyArea
        {
            /// <summary>
            /// The RenderableKeyLayout.KeyArea being built
            /// </summary>
            public readonly RKeyArea renderable;

            private List<RKey> keys = new List<RKey>();

            public Rect rect
            {
                get { return renderable.rect; }
                set { renderable.rect = value; }
            }

            public BKeyArea(RKeyArea renderable)
            {
                this.renderable = renderable;
            }

            /// <summary>
            /// Add a key to this key area
            /// </summary>
            /// <param name="key">The renderable key to add</param>
            public void AddKey(RKey key)
            {
                keys.Add(key);
            }

            /// <summary>
            /// Commit all keys to the renderable key area
            /// </summary>
            /// <remarks>This is implicitly called by `BKeyset.Commit`</remarks>
            public void Commit()
            {
                this.renderable.keys = keys.ToArray();
            }
        }

        /// <summary>
        /// A high-level builder meta class exposing KeyboardLayout.Row instances
        /// </summary>
        protected class BRow
        {
            /// <summary>
            /// The row index
            /// </summary>
            public readonly int index;
            /// <summary>
            /// The original KeyboardLayout.Row
            /// </summary>
            public readonly IRow<IKey> raw;

            public int keyCount
            {
                get { return raw.GetKeys().Length; }
            }

            public BRow(int index, IRow<IKey> raw)
            {
                this.index = index;
                this.raw = raw;
            }

            protected int GetSplitIndex()
            {
                ISplittable splittableRow = raw as ISplittable;
                if (splittableRow != null)
                {
                    return splittableRow.GetSplitIndex();
                }
                return 0;
            }

            /// <summary>
            /// Return an array of high-level builders for the keys in this row
            /// </summary>
            /// <returns>High-level key builders</returns>
            public BKey[] Keys()
            {
                IKey[] rawKeys = raw.GetKeys();
                BKey[] keys = new BKey[rawKeys.Length];
                for (int k = 0; k < rawKeys.Length; k++)
                {
                    keys[k] = new BKey(k, k, rawKeys[k]);
                }
                return keys;
            }

            /// <summary>
            /// Return an array of high-level builders for the keys on the left side of this row
            /// </summary>
            /// <returns>High-level key builders</returns>
            public BKey[] LeftKeys()
            {
                int splitIndex = GetSplitIndex();
                IKey[] rawKeys = raw.GetKeys();
                BKey[] keys = new BKey[splitIndex];
                for (int k = 0; k < splitIndex; k++)
                {
                    keys[k] = new BKey(k, k, rawKeys[k]);
                }
                return keys;
            }

            /// <summary>
            /// Return an array of high-level builders for the keys on the right side of this row
            /// </summary>
            /// <returns>High-level key builders</returns>
            public BKey[] RightKeys()
            {
                int splitIndex = GetSplitIndex();
                IKey[] rawKeys = raw.GetKeys();
                BKey[] keys = new BKey[rawKeys.Length - splitIndex];
                for (int k = splitIndex; k < rawKeys.Length; k++)
                {
                    keys[k - splitIndex] = new BKey(k, k - splitIndex, rawKeys[k]);
                }
                return keys;
            }
        }

        /// <summary>
        /// A high-level builder meta class exposing KeyboardLayout.Key instances
        /// </summary>
        protected class BKey
        {
            /// <summary>
            /// The key index within the row
            /// </summary>
            public readonly int index;
            /// <summary>
            /// The key index within the local half of the row
            /// </summary>
            public readonly int localIndex;
            /// <summary>
            /// The original KeyboardLayout.Key
            /// </summary>
            public readonly IKey raw;

            public int weight
            {
                get
                {
                    IWeighted weightedKey = raw as IWeighted;
                    if (weightedKey != null)
                    {
                        int weight = weightedKey.GetWeight();
                        return weight < 1 ? 1 : weight;
                    }
                    return 1;
                }
            }

            public bool isSpecial
            {
                get
                {
                    return raw.GetKeyClass() != KeyClass.Character;
                }
            }

            public BKey(int index, int localIndex, IKey raw)
            {
                this.index = index;
                this.localIndex = localIndex;
                this.raw = raw;
            }

            /// <summary>
            /// Create a RenderableKeyLayout.Key based off this key's metadata
            /// </summary>
            /// <returns>The renderable key</returns>
            public RKey Create()
            {
                RKey rKey = new RKey();

                rKey.keyClass = raw.GetKeyClass();
                switch (rKey.keyClass)
                {
                    case KeyClass.Character:
                        rKey.character = raw.GetCharacter();
                        rKey.label = rKey.name = rKey.character.ToString();
                        if (rKey.character == ' ')
                        {
                            rKey.name = "Spacebar";
                        }
                        break;
                    case KeyClass.KeysetModifier:
                        rKey.keyset = raw.GetKeyset();
                        rKey.name = "KeysetModifier";
                        rKey.label = ""; // We do not have enough information to set this now
                        break;
                    case KeyClass.Backspace:
                        rKey.label = rKey.name = "Backspace";
                        break;
                    case KeyClass.Enter:
                        rKey.label = rKey.name = "Enter";
                        break;
                }

                return rKey;
            }

            /// <summary>
            /// Calculate the total weight of an array of keys
            /// </summary>
            /// <param name="keys">Keys to calculate the weight of</param>
            public static int RowWeight(BKey[] keys)
            {
                int weight = 0;
                foreach (BKey key in keys)
                {
                    weight += key.weight;
                }
                return weight;
            }
        }

        /// <summary>
        /// Return a Renderable Keyboard Layout builder for the keyboard layout attached to this component
        /// </summary>
        /// <returns></returns>
        protected RKeyboardBuilder BuildKeyboard()
        {
            return new RKeyboardBuilder((IKeyLayout<IRowKeyset<IRow<IKey>, IKey>>)keyboardLayout);
        }

        /// <summary>
        /// Base Keyboard Layout Calculator implements this to provide the higher-level `CalculateKeyset` builder interface
        /// </summary>
        /// <param name="containerSize">The dimensions of the container to render into</param>
        /// <returns>The generated renderable key layout</returns>
        public override RKeyLayout CalculateKeyLayout(Vector2 containerSize)
        {
            if (keyboardLayout == null)
            {
                Debug.LogError("Keyboard Layout Calculator in " + name + " requires a Keyboard Layout");
                return null;
            }

            RKeyboardBuilder b = BuildKeyboard();

            foreach (BKeyset keyset in b.Keysets())
            {
                CalculateKeyset(keyset, containerSize);
            }

            return b.Commit();
        }

        /// <summary>
        /// Keyboard Layout Calculators can override CalculateKeyset to build a keyboard layout using a high level builder
        /// </summary>
        /// <param name="keyset">Keyboard layout keyset builder</param>
        /// <param name="containerSize">Dimensions of the container</param>
        protected virtual void CalculateKeyset(BKeyset keyset, Vector2 containerSize)
        {
            throw new System.NotImplementedException(typeof(VRTK_BaseKeyboardLayoutCalculator) + " subclasses must override either the CalculateKeyLayout or CalculateKeyset method");
        }
    }
}
