namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

public interface IEntity<out TKey>
    where TKey : struct
{
    TKey Id { get; }

    string ToLogString(string val = "");
}