using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arithmetic 
{
    public static Arithmetic instance;
    public static Arithmetic Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Arithmetic();
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    public int Partition(int[] _arr, int _left, int _right)
    {
        int key = _left;
        while (_left < _right)
        {
            while (_left < _right && _arr[_right] <= _arr[key])
            {
                _right -= 1;
            }
            while (_left < _right && _arr[_left] >= _arr[key])
            {
                _left += 1;
            }
            (_arr[_left], _arr[_right]) = (_arr[_right], _arr[_left]);
        }
       
        (_arr[_left], _arr[key]) = (_arr[key], _arr[_left]);

        return _left;
    }
    public void QuickSort(int[] _arr, int _left, int _right)
    {
        if (_left >= _right) return;
        int mid = Partition(_arr, _left, _right);
        QuickSort(_arr, _left, mid - 1);
        QuickSort(_arr, mid + 1, _right);
    }
}
