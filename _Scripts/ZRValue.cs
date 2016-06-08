using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ZRSettings
{
    private ZRValue hp = ZRValue.zero;
    private ZRValue cage = ZRValue.zero;
    private ZRValue reduction = ZRValue.zero;
    private ZRValue speed = ZRValue.zero;

    public List<ZRSetting>  names = new List<ZRSetting>();
    public List<ZRValue>    values = new List<ZRValue>();
    public int count
    {
        get
        {
            return names.Count;
        }
    }

    public ZRValue this[ZRSetting _name, bool init = false]
    {
        get
        {
            if (!init)
            {
                switch (_name)
                {
                    case ZRSetting.HP:
                        return hp;
                    case ZRSetting.Cage:
                        return cage;
                    case ZRSetting.Reduction:
                        return reduction;
                    case ZRSetting.Speed:
                        return speed;

                }
            }
           
            if (!Contains(_name))
            {
                return new ZRValue(0);
            }
            
            return values[(names.IndexOf(_name))];
        }
        set
        {
            if (!init)
            {
                switch (_name)
                {
                    case ZRSetting.HP:
                        hp = value;
                        return;
                    case ZRSetting.Cage:
                        cage = value;
                        return;
                    case ZRSetting.Reduction:
                        reduction = value;
                        return;
                    case ZRSetting.Speed:
                        speed = value;
                        return;

                }
            }
            if (!names.Contains(_name))
            {
                Add(_name, value);                                
            }
            if (value != null)
            {
                values[(names.IndexOf(_name))] = value;
            }
            else
            {
                Remove(_name);
            }
        }
    }

   
    public void Init()
    {
        hp = this[ZRSetting.HP, true];
        cage = this[ZRSetting.Cage, true];
        reduction = this[ZRSetting.Reduction, true];
        speed = this[ZRSetting.Speed, true];
    }

    public ZRSettings Clone()
    {
        ZRSettings setts = new ZRSettings();
        for (int i = 0; i < count; i++)
        {
            setts.Add(names[i], values[i].Clone());
        }
        setts.Init();
        return setts;
    }

    

    public void Add(ZRSetting _name, ZRValue _value)
    {
        names.Add(_name);
        values.Add(_value);
        Init();
    }

    public bool Contains(ZRSetting setting)
    {
        return names.Contains(setting);
    }

    public static ZRSettings operator +(ZRSettings one, ZRSettings two)
    {
        ZRSettings settings = new ZRSettings();
        for (int i = 0; i < one.count; i++)
        {
            if (!settings.Contains(one.names[i]))
            {
                settings.Add(one.names[i], new ZRValue(0));
            }
            settings[one.names[i]] = one.values[i] + two[one.names[i]];
        }
        for (int i = 0; i < two.count; i++)
        {
            if (settings.Contains(two.names[i]))
            {
                continue;
            }
            settings.Add(two.names[i], two.values[i]);
        }
        settings.Init();
        return settings;
    }

    public static ZRSettings operator *(ZRSettings one, float value)
    {
        ZRSettings settings = new ZRSettings();
        for (int i = 0; i < one.count; i++)
        {
            settings.Add(one.names[i], one.values[i] * value);
        }
        settings.Init();
        return settings;
    }

    public static ZRSettings operator /(ZRSettings one, float value)
    {
        ZRSettings settings = new ZRSettings();
        for (int i = 0; i < one.count; i++)
        {
            settings.Add(one.names[i], one.values[i] / value);
        }
        settings.Init();
        return settings;
    }

    public static ZRSettings operator -(ZRSettings one, ZRSettings two)
    {
        ZRSettings settings = new ZRSettings();
        for (int i = 0; i < one.count; i++)
        {

            if (!settings.Contains(one.names[i]))
            {
                settings.Add(one.names[i], new ZRValue(0));
            }
            settings[one.names[i]] = one.values[i] - two[one.names[i]];
        }
        for (int i = 0; i < two.count; i++)
        {
            if (settings.Contains(two.names[i]))
            {
                continue;
            }
            settings.Add(two.names[i], two.values[i] * (-1));

        }
        settings.Init();
        return settings;
    }

    public void Multyply(float koef)
    {
        for (int i = 0; i < count; i++)
        {
            this.values[i] *= koef;
        }
        Init();
    }

    public void Multyply(ZRSettings sets)
    {
        for (int i = 0; i < count; i++)
        {
            if (sets.names.Contains(this.names[i]))
            {
                this.values[i] *= sets[this.names[i]].percent;
            }
        }
        Init();
    }

    public void Addition(ZRSettings sets)
    {
        for (int i = 0; i < count; i++)
        {
            if (sets.names.Contains(this.names[i]))
            {
                ZRValue val = this.values[i] * sets[this.names[i]].percent;
                
                if (this.names[i] == ZRSetting.Cage)
                {
                    val.ToInt();
                }
                this.values[i] += val;
            }
        }
        Init();
    }

    public void Contact(ZRSettings sets)
    {
        for (int i = 0; i < sets.count; i++)
        {
            if (this.Contains(sets.names[i]))
            {
                continue;
            }
            this.Add(sets.names[i], sets.values[i]);
        }
    }

    public void Koef(ZRSettings sets)
    {
        for (int i = 0; i < count; i++)
        {
            if (!sets.names.Contains(this.names[i]))
            {
                continue;
            }
            ZRValue val = this.values[i];
            
            val.current += val.max * sets[this.names[i]].percent;
            //Debug.Log(this.names[i] + ":" + sets[this.names[i]].percent + ": " + sets[this.names[i]]);
            if (val.peaked)
            {
                val.Refresh();
            }
            if (val.minimum)
            {
                val.Reset();
            }
            if (this.names[i] == ZRSetting.Cage)
            {
                val.current = (int)val.current;
                //Debug.Log(val.ToString());
            }

        }
        Init();
    }


    public void Remove(ZRSetting _name)
    {
        values.RemoveAt(names.IndexOf(_name));
        names.Remove(_name);
        Init();
    }

    public void Replace(ZRSettings sets)
    {
        for (int i = 0; i < count; i++)
        {
            if (sets.names.Contains(this.names[i]))
            {
                this.values[i] = sets[this.names[i]];
            }
        }
    }

    public override string ToString()
    {
        string res = "";
        for (int i = 0; i < this.count; i++)
        {
            res += this.names[i].ToString() + ": " + this.values[i].ToString() + "\n";
        }
        return res;
    }

}

public enum ZRSetting
{
    /// <summary>
    /// Здоровье
    /// </summary>
    HP,
    /// <summary>
    /// Длительность
    /// </summary>
    Duration,
    /// <summary>
    /// Урон
    /// </summary>
    Damage,
    /// <summary>
    /// Скорострельность
    /// </summary>
    RPM,
    /// <summary>
    /// Выстелы в минуту
    /// </summary>   
    AttackAngle,
    /// <summary>
    /// Дальность атаки
    /// </summary>
    AttackLength,
    /// <summary>
    /// Скорость
    /// </summary>
    Speed,
    /// <summary>
    /// Ускорение
    /// </summary>
    Accelerate,
    /// <summary>
    /// Сфера действия
    /// </summary>
    Scope,
    /// <summary>
    /// Обойма, либо статус орудия
    /// </summary>
    Cage,   
    /// <summary>
    /// Затраты (либо сила нагрева, либо количество снарядов за один выстрел)
    /// </summary>
    Spending,
    /// <summary>
    /// Скорость восстановления (% в секунду)
    /// </summary>
    Reduction,
    /// <summary>
    /// Скорость поворота
    /// </summary>
    AngularSpeed,
    /// <summary>
    /// Масса
    /// </summary>
    Mass,
    /// <summary>
    /// Вероятность критического попадания
    /// </summary>
    CriticalStrike,   
    /// <summary>
    /// Защита
    /// </summary>
    Armor,
    /// <summary>
    /// Количество
    /// </summary>
    Count,
    /// <summary>
    /// Топливо
    /// </summary>
    Fuel,
}


[System.Serializable]
public class ZRValue
{
    public float min;
    public float current;
    public float max;

    public float percent
    {
        get
        {
            if ((max - min) != 0 && (max > min))
            {
                return ((current - min) / (max - min));
            }
            else
            {
                return 0;
            }
        }
    }

    public bool minimum
    {
        get
        {
            return min >= current;
        }
    }

    public bool peaked
    {
        get
        {
            return max <= current;
        }
    }

    public static ZRValue zero
    {
        get
        {
            return new ZRValue(0);
        }
    }

    public ZRValue Clone()
    {
        return new ZRValue(min, current, max);
    }

    public ZRValue()
    {
        min = 0;
        current = 1;
        max = 1;
    }

    public ZRValue(float _value)
    {
        min = 0;
        current = _value;
        max = _value;

    }

    public ZRValue(float _current, float _max)
    {
        min = 0;
        current = _current;
        max = _max;

    }

    public ZRValue(float _min, float _current, float _max)
    {
        min = _min;
        current = _current;
        max = _max;
    }

    public void Refresh()
    {
        current = max;
    }

    public void Reset()
    {
        current = min;
    }

    public float RandomValue()
    {
        return Random.Range(min, max);
    }

    public static ZRValue operator +(ZRValue _value1, ZRValue _value2)
    {
        return new ZRValue(_value1.min + _value2.min, _value1.current + _value2.current, _value1.max + _value2.max);
    }

    public static ZRValue operator *(ZRValue _value1, float _value2)
    {
        return new ZRValue(_value1.min * _value2, _value1.current * _value2, _value1.max * _value2);
    }

    public static ZRValue operator /(ZRValue _value1, float _value2)
    {
        return new ZRValue(_value1.min / _value2, _value1.current / _value2, _value1.max / _value2);
    }

    public static ZRValue operator -(ZRValue _value1, ZRValue _value2)
    {
        return new ZRValue(_value1.min - _value2.min, _value1.current - _value2.current, _value1.max - _value2.max);
    }

    public static ZRValue operator +(ZRValue _value1, float _value2)
    {
        return new ZRValue(_value1.min, _value1.current + _value2, _value1.max);
    }

    public static ZRValue operator -(ZRValue _value1, float _value2)
    {
        return new ZRValue(_value1.min, _value1.current - _value2, _value1.max);
    }

    public static implicit operator float(ZRValue value)
    {
        return value.current;
    }

    public static implicit operator Vector2(ZRValue value)
    {
        return new Vector2(value.current, value.max);
    }

    public static implicit operator ZRValue(Vector2 value)
    {
        return new ZRValue(value.x, value.y);
    }

    public void ToInt()
    {
        min = Mathf.CeilToInt(min);
        current = Mathf.CeilToInt(current);
        max = Mathf.CeilToInt(max);
    }
         

    public override string ToString()
    {
        return current.ToString() + " | " + max.ToString();
    }
}

