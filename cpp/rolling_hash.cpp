struct RollingHash
{
private:
    ll _hashA;
    ll _hashB;
    int _length;

public:
    RollingHash()
    {
        _hashA = 0;
        _hashB = 0;
        _length = 0;
    }

    RollingHash(ll hashA, ll hashB, int length)
    {
        _hashA = hashA;
        _hashB = hashB;
        _length = length;
    }

    ll HashA()
    {
        return _hashA;
    }

    ll HashB()
    {
        return _hashB;
    }

    int Length()
    {
        return _length;
    }

    bool operator ==(const RollingHash& other)
    {
        return _hashA == other._hashA && _hashB == other._hashB && _length == other._length;
    }

    inline static RollingHash Zero()
    {
        return RollingHash(0, 0, 0);
    }

    inline static RollingHash FromChar(char c)
    {
        return RollingHash((ll)c, (ll)c, 1);
    }
};

class RabinKarp
{
public:
    static const ll BaseA = 3491;
    static const ll ModA = 481840747;
    static const ll BaseB = 8761;
    static const ll ModB = 999750347;

private:
    static vector<ll> _powerA;
    static vector<ll> _powerB;

public:
    static inline ll SafeMod(ll a, ll m)
    {
        return ((a % m) + m) % m;
    }

    static void Setup(int maxLength)
    {
        _powerA.resize(maxLength + 1);
        _powerB.resize(maxLength + 1);
        _powerA[0] = 1LL;
        _powerB[0] = 1LL;
        for (int i = 1; i <= maxLength; i++)
        {
            _powerA[i] = (_powerA[i - 1] * BaseA) % ModA;
            _powerB[i] = (_powerB[i - 1] * BaseB) % ModB;
        }
    }

    static RollingHash PrefixDiff(RollingHash left, RollingHash right)
    {
        int diff = right.Length() - left.Length();
        ll hashA = SafeMod(right.HashA() - (left.HashA() * _powerA[diff]) % ModA, ModA);
        ll hashB = SafeMod(right.HashB() - (left.HashB() * _powerB[diff]) % ModB, ModB);

        return RollingHash(hashA, hashB, diff);
    }

    static RollingHash Concat(RollingHash left, RollingHash right)
    {
        int len = left.Length() + right.Length();
        ll hashA = ((left.HashA() * _powerA[right.Length()]) % ModA + right.HashA()) % ModA;
        ll hashB = ((left.HashB() * _powerB[right.Length()]) % ModB + right.HashB()) % ModB;
        return RollingHash(hashA, hashB, len);
    }

    static RollingHash HashFromString(string s)
    {
        ll hashA = 0LL;
        ll hashB = 0LL;
        for (int i = 0; i < s.size(); i++)
        {
            hashA = (hashA * BaseA) % ModA;
            hashA += (ll)s[i];
            hashA %= ModA;

            hashB = (hashB * BaseB) % ModB;
            hashB += (ll)s[i];
            hashB %= ModB;
        }

        return RollingHash(hashA, hashB, (int)s.size());
    }

    inline static ll PowerA(int exp)
    {
        return _powerA[exp];
    }

    inline static ll PowerB(int exp)
    {
        return _powerB[exp];
    }
};

vector<ll> RabinKarp::_powerA;
vector<ll> RabinKarp::_powerB;

class RollingHashString
{
private:
    int _length;
    string _source;
    vector<RollingHash> _prefix;

public:
    RollingHashString(string source)
    {
        assert(source.size() != 0);
        _source = source;
        _length = (int)source.size();
        // pre-compute prefix hashes

        _prefix.resize(_length + 1);
        _prefix[0] = RollingHash::Zero();
        for (int i = 1; i <= _length; i++)
        {
            _prefix[i] = RabinKarp::Concat(_prefix[i - 1], RollingHash::FromChar(_source[i - 1]));
        }
    }

    RollingHash GetHash(int l, int r)
    {
        assert(0 <= l && l < _length && l <= r && r <= _length);
        return RabinKarp::PrefixDiff(_prefix[l], _prefix[r]);
    }

    RollingHash GetPrefixHash(int length)
    {
        assert(0 <= length && length <= _length);
        return _prefix[length];
    }

    string Source()
    {
        return _source;
    }

    int Length()
    {
        return _length;
    }
};