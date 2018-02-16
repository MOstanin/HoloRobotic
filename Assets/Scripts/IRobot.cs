using MathNet.Numerics.LinearAlgebra;


public interface IRobot
{
    Matrix<float> ForwardKin(float[] q);
    float[] InversKin(Matrix<float> Tgoal, float[] q0);
    void SendState(float[] q);
    float[] ReadState();
    void Move();
}
