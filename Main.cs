using Godot;
using System;

public partial class Main : Node2D, ObjectPoolReturnHandler<Ball>, ObjectPoolReturnHandler<Brick>
{
	private Paddle paddle;
	private ObjectPool<Ball> ballPool;
	private ObjectPool<Brick> brickPool;

	private int lives = 3;

    public void Return(Ball obj)
    {
        ballPool.Return(obj);
		GD.Print($"Ball returned to pool. Active balls: {ballPool.Active}, Inactive balls: {ballPool.Inactive}");
    }

    public void Return(Brick obj)
    {
        brickPool.Return(obj);
		GD.Print($"Brick returned to pool. Active bricks: {brickPool.Active}, Inactive bricks: {brickPool.Inactive}");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PackedScene ballScene = GD.Load<PackedScene>("res://ball.tscn");
		ballPool = new ObjectPool<Ball>(ballScene, 3, this);
		PackedScene brickScene = GD.Load<PackedScene>("res://brick.tscn");
		brickPool = new ObjectPool<Brick>(brickScene, 10, this);

		PackedScene paddleScene = GD.Load<PackedScene>("res://paddle.tscn");
		paddle = paddleScene.Instantiate<Paddle>();
		AddChild(paddle);
		paddle.Position = new Vector2(640,650);

		StaticBody2D topWall = GetNode<StaticBody2D>("TopWall");
		StaticBody2D leftWall = GetNode<StaticBody2D>("LeftWall");
		StaticBody2D rightWall = GetNode<StaticBody2D>("RightWall");
		Area2D deadZone = GetNode<Area2D>("DeadZone");
		deadZone.BodyEntered += OnDeadZoneBodyEntered;


		//test section
		Ball ball = ballPool.Get();
		ball.AttachTo(paddle);
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				Brick brick = brickPool.Get();
				brick.Position = new Vector2(300 + i * 80, 200 + j * 50);
			}
		}
	}

	private bool GameOverCheck()
	{
		if (lives <= 0)
		{
			GD.Print("Game Over!");
			// Additional game over logic here
			return true;
		}
		return false;
	}

	private bool VictoryCheck()
	{
		if (brickPool.Active == 0)
		{
			GD.Print("You Win!");
			// Additional victory logic here
			return true;
		}
		return false;
	}

	private bool lostLifeCheck()
	{
		if (ballPool.Active == 0)
		{
			GD.Print("Life lost!");
			return true;
		}
		return false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (lostLifeCheck())
		{
			GD.Print($"Lives remaining: {lives - 1}");
			lives--;
			SpawnBall();
		}
		if (GameOverCheck() || VictoryCheck())
		{
			GetTree().Quit();
		}
	}


	public void SpawnBall()
	{
		// Implementation for spawning a new ball
		Ball ball = ballPool.Get();
		ball.AttachTo(paddle);
	}

	private void OnDeadZoneBodyEntered(Node2D body)
	{
		if (body is Ball ball)
		{
			GD.Print("Ball entered dead zone!");
			ball.OnDespawn();
			ballPool.Return(ball);
		}
	}
}