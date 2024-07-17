using Godot;
using System;

public partial class Car_2 : CharacterBody2D
{
    Vector2 CarLocation;
    float CarHeading;
    float CarSpeed;
    float SteerAngle = 15;
    float SteerDirection;
    float WheelBase = 70; // the distance between the two axles

    int EnginePower = 1000;
    float Friction = -0.9f;
    float Drag = -0.0001f;
    Vector2 Acceleration = Vector2.Zero;
    int Braking = -450;
    int MaxSpeedReverse = 250;

    //Speed above you start sliding/drifting
    int SlipSpeed = 400;
    //How slippery it is when above slip speed
    float TractionFast = 0.1f;
    //How slippery it is when below slip speed
    float TractionSlow = 0.7f;

    public override void _Ready()
    {
        base._Ready();
        Velocity = Vector2.Zero;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Acceleration = Vector2.Zero;

        //Get Input
        GetInput();

        //Apply Friction
        ApplyFriction();

        //Calculate Steering
        CalculateSteering(delta);

        Velocity += Acceleration * (float)delta;

        MoveAndSlide();
    }

    public void ApplyFriction()
    {
        if (Velocity.Length() < 5)
        {
            Velocity = Vector2.Zero;
        }
        var frictionForce = Velocity * Friction;
        var dragForce = Velocity * Velocity.Length() * Drag;
        Acceleration += dragForce + frictionForce;
    }

    public void GetInput()
    {
        var turn = 0;
        if (Input.IsActionPressed("turnRight_player2"))
        {
            ++turn;
        }

        if (Input.IsActionPressed("turnLeft_player2"))
        {
            --turn;
        }

        SteerDirection = turn * Mathf.DegToRad(SteerAngle);

        if (Input.IsActionPressed("accelerate_player2"))
        {
            Acceleration = Transform.X * EnginePower;
        }

        if (Input.IsActionPressed("brake_player2"))
        {
            Acceleration = Transform.X * Braking;
        }
    }

    public void CalculateSteering(double delta)
    {
        //Find the wheel positions
        var rearWheel = Position - Transform.X * WheelBase / 2;
        var frontWheel = Position + Transform.X * WheelBase / 2;

        //Move the wheels forward
        rearWheel += Velocity * (float)delta;
        frontWheel += Velocity.Rotated(SteerDirection) * (float)delta;

        //Find the new direction vector
        var newHeading = (frontWheel - rearWheel).Normalized();

        var traction = TractionSlow;
        if (Velocity.Length() > SlipSpeed)
        {
            traction = TractionFast;
        }

        //Set the Velocity to new direction depending on if you're acceleration or reversing
        var dotProduct = newHeading.Dot(Velocity.Normalized());

        if (dotProduct > 0)
        {
            Velocity = Velocity.Lerp(newHeading * Velocity.Length(), traction);
        }
        if (dotProduct < 0)
        {
            Velocity = -newHeading * Math.Min(Velocity.Length(), MaxSpeedReverse);
        }

        //4. Set the rotation to the new direction
        Rotation = newHeading.Angle();
    }
}
