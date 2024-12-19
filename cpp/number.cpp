class Number
{
public:
    static ll SafeMod(ll x, ll m)
    {
        x %= m;
        if (x < 0)
            x += m;
        return x;
    }

    static ll ExtEuclid(ll a, ll b, ll& p, ll& q)
    {
        if (b == 0)
        {
            p = 1;
            q = 0;
            return a;
        }
        ll d = ExtEuclid(b, a % b, q, p);
        q -= a / b * p;
        return d;
    }

    static pair<ll, ll> CRT(ll x1, ll m1, ll x2, ll m2)
    {
        ll p, q;
        ll d = ExtEuclid(m1, m2, p, q);
        if ((x2 - x1) % d != 0)
            return make_pair(0, -1);

        ll m = m1 * (m2 / d);
        ll temp = (x2 - x1) / d * p % (m2 / d);
        ll r = SafeMod(x1 + m1 * temp, m);
        return make_pair(r, m);
    }

    static pair<ll, ll> CRT(vector<ll>& x, vector<ll>& mod)
    {
        ll r = 0, m = 1;
        for (int i = 0; i < (int)x.size(); i++)
        {
            ll p, q;
            ll d = ExtEuclid(m, mod[i], p, q);
            if ((x[i] - r) % d != 0)
                return make_pair(0, -1);
            ll temp = (x[i] - r) / d * p % (mod[i] / d);
            r += m * temp;
            m *= mod[i] / d;
        }

        return make_pair(SafeMod(r, m), m);
    }
};