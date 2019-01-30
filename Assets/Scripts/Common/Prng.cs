using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Prng {

    // State for generating numbers
    private uint State;
 
    // Get current number
    public uint GetState() => State;

    // Initialise Prng
    public void Seed(uint seedValue) => State = seedValue;

    //  Give next Random 
    private uint XorShift()
    {
        // get the value
        uint x = State;
        // perform Xor shifting
        x ^= x >> 17;
        x ^= x << 5;
        // and return new state
        State = x;
        return x;
    }

    /*
     * Return various pseudo random Results
     */
    public int NextInt() => (int)XorShift();
    public bool NextBool() => ((XorShift() % 2) == 1) ? true : false;
    public uint NextUint() => XorShift();
    public float NextPercent() => Mathf.Abs((int)XorShift() % 100) /100;

    // public int NextRange(int min, int max) => Mathf.Abs(((int)XorShift() % max) + min);

    public int NextRange(int min, int max)
    {
        int l = (int)XorShift() % max;
        return Mathf.Abs(l + min);
    }

    /*
     * NextFloat :
     *      Given value between 0.0f and 1.0f
     */
    public float NextFloat()
    {
        float max = uint.MaxValue;
        float n = (int)XorShift();
        return Mathf.Abs((n / max)*2);
    }

    /*
     * NextDirection :
     *      One particular direction
     */
    public Edirection NextDirection()
    {
        int d = (int)XorShift() % 4;
        switch (d)
        {
            case 1: return Edirection.Up; 
            case 2: return Edirection.Right;
            case 3: return Edirection.Down;
        }
        return Edirection.Down;
    }
        
}
