using System;
using System.Collections.Generic;
using RDtext.Attributes;

namespace RDtext.Options {

    /// <summary>
    ///     base class for an option set
    /// </summary>
    /// <typeparam name="TSet"></typeparam>
    [Mutable]
    public abstract class OptionSet<TSet> : OptionBase where TSet : OptionSet<TSet> {

        private readonly Lazy<IList<OptionBase>> allOptions;

        /// <summary>
        ///     create a new option set
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        protected OptionSet(string name, OptionSet<TSet>? parent) : base(name, parent)
            => allOptions = new Lazy<IList<OptionBase>>(() => GetAllOptions(), true);

        /// <summary>
        ///     get all options
        /// </summary>
        /// <returns></returns>
        private IList<OptionBase> GetAllOptions() {
            var result = new List<OptionBase>();
            result.AddRange(Options);
            result.Sort();
            return result;
        }

        /// <summary>
        ///     all options
        /// </summary>
        public IList<OptionBase> AllOptions
            => allOptions.Value;

        /// <summary>
        ///     get an option by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public OptionBase? GetOptionByName(string name) {
            var options = AllOptions;

            foreach (var option in options)
                if (string.Equals(name, option.Name, StringComparison.Ordinal))
                    return option;

            return default;
        }

        /// <summary>
        ///     list all options
        /// </summary>
        protected abstract IEnumerable<OptionBase> Options { get; }

        /// <summary>
        ///     check for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(OptionBase? other) {
            if (other is not OptionSet<TSet> set)
                return false;

            if (set.AllOptions.Count != AllOptions.Count)
                return false;

            foreach (var option in AllOptions)
                if (!option.Equals(set.GetOptionByName(option.Name)))
                    return false;

            return true;
        }

        /// <summary>
        ///     compute a hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            var hc = new HashCode();
            hc.Add(Name, StringComparer.Ordinal);
            foreach (var option in AllOptions)
                hc.Add(option.GetHashCode());
            return hc.ToHashCode();
        }
    }
}
