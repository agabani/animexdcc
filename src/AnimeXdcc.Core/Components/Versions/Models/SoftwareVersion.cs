using System;
using System.Globalization;

namespace AnimeXdcc.Core.Components.Versions.Models
{
    public class SoftwareVersion : ICloneable, IComparable, IComparable<SoftwareVersion>, IEquatable<SoftwareVersion>
    {
        private static readonly char[] SeparatorArray = {'.'};

        public SoftwareVersion(long major, long minor, long patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public long Major { get; private set; }
        public long Minor { get; private set; }
        public long Patch { get; private set; }

        public Object Clone()
        {
            return new SoftwareVersion(Major, Minor, Patch);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var softwareVersion = obj as SoftwareVersion;

            return CompareTo(softwareVersion);
        }

        public int CompareTo(SoftwareVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            if (Major != other.Major)
            {
                if (Major > other.Major)
                {
                    return 1;
                }
                return -1;
            }

            if (Minor != other.Minor)
            {
                if (Minor > other.Minor)
                {
                    return 1;
                }
                return -1;
            }

            if (Patch != other.Patch)
            {
                if (Patch > other.Patch)
                {
                    return 1;
                }
                return -1;
            }

            return 0;
        }

        public bool Equals(SoftwareVersion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }

        public override bool Equals(Object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((SoftwareVersion) other);
        }

        public override int GetHashCode()
        {
            long hashCode = 0;

            hashCode |= (Major & 0x0000000F) << 28;
            hashCode |= (Minor & 0x000000FF) << 20;
            hashCode |= (Patch & 0x000000FF) << 12;

            return (int) hashCode;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
        }

        public static SoftwareVersion Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var softwareVersionResult = new SoftwareVersionResult();
            softwareVersionResult.Init("input", true);
            if (!TryParseSoftwareVersion(input, ref softwareVersionResult))
            {
                throw softwareVersionResult.GetSoftwareVersionParseException();
            }
            return softwareVersionResult.ParsedSoftwareVersion;
        }

        public static bool TryParse(string input, out SoftwareVersion result)
        {
            var versionResult = new SoftwareVersionResult();
            versionResult.Init("input", false);
            var success = TryParseSoftwareVersion(input, ref versionResult);
            result = versionResult.ParsedSoftwareVersion;
            return success;
        }

        private static bool TryParseSoftwareVersion(string softwareVersion, ref SoftwareVersionResult result)
        {
            long major, minor, patch = 0;

            if (softwareVersion == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNullException);
                return false;
            }

            var parsedComponents = softwareVersion.Split(SeparatorArray);
            var parsedComponentsLength = parsedComponents.Length;
            if (parsedComponentsLength < 2 || parsedComponentsLength > 3)
            {
                result.SetFailure(ParseFailureKind.ArgumentException);
                return false;
            }

            if (!TryParseComponent(parsedComponents[0], "major", ref result, out major))
            {
                return false;
            }

            if (!TryParseComponent(parsedComponents[1], "minor", ref result, out minor))
            {
                return false;
            }

            if (parsedComponentsLength > 2)
            {
                if (!TryParseComponent(parsedComponents[2], "patch", ref result, out patch))
                {
                    return false;
                }
            }

            result.ParsedSoftwareVersion = new SoftwareVersion(major, minor, patch);

            return true;
        }

        private static bool TryParseComponent(string component, string componentName, ref SoftwareVersionResult result,
            out long parsedComponent)
        {
            if (!long.TryParse(component, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedComponent))
            {
                result.SetFailure(ParseFailureKind.FormatException, component);
                return false;
            }

            if (parsedComponent < 0)
            {
                result.SetFailure(ParseFailureKind.ArgumentOutOfRangeException, componentName);
                return false;
            }

            return true;
        }

        public static bool operator ==(SoftwareVersion v1, SoftwareVersion v2)
        {
            if (ReferenceEquals(v1, null))
            {
                return ReferenceEquals(v2, null);
            }

            return v1.Equals(v2);
        }

        public static bool operator !=(SoftwareVersion v1, SoftwareVersion v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(SoftwareVersion v1, SoftwareVersion v2)
        {
            if ((object) v1 == null)
            {
                throw new ArgumentNullException("v1");
            }

            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >(SoftwareVersion v1, SoftwareVersion v2)
        {
            return v2 < v1;
        }

        public static bool operator <=(SoftwareVersion v1, SoftwareVersion v2)
        {
            if ((object) v1 == null)
            {
                throw new ArgumentNullException("v1");
            }

            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >=(SoftwareVersion v1, SoftwareVersion v2)
        {
            return v2 <= v1;
        }

        internal enum ParseFailureKind
        {
            ArgumentNullException,
            ArgumentException,
            ArgumentOutOfRangeException,
            FormatException
        }

        internal struct SoftwareVersionResult
        {
            internal string ArgumentName;
            internal bool CanThrow;
            internal string ExceptionArgument;
            internal ParseFailureKind Failure;
            internal SoftwareVersion ParsedSoftwareVersion;

            internal void Init(string argumentName, bool canThrow)
            {
                ArgumentName = argumentName;
                CanThrow = canThrow;
            }

            internal void SetFailure(ParseFailureKind failure)
            {
                SetFailure(failure, string.Empty);
            }

            internal void SetFailure(ParseFailureKind failure, string argument)
            {
                Failure = failure;
                ExceptionArgument = argument;
                if (CanThrow)
                {
                    throw GetSoftwareVersionParseException();
                }
            }

            internal Exception GetSoftwareVersionParseException()
            {
                switch (Failure)
                {
                    case ParseFailureKind.ArgumentNullException:
                        return new ArgumentNullException(ArgumentName);
                    case ParseFailureKind.ArgumentException:
                        return new ArgumentException(ArgumentName);
                    case ParseFailureKind.ArgumentOutOfRangeException:
                        return new ArgumentOutOfRangeException(ArgumentName);
                    case ParseFailureKind.FormatException:
                        try
                        {
                            long.Parse(ExceptionArgument, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException exception)
                        {
                            return exception;
                        }
                        catch (OverflowException exception)
                        {
                            return exception;
                        }
                        return new FormatException();
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}