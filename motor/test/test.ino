

#define STEER_PIN_1 22
#define STEER_PIN_2 23
#define THROTTLE_PIN_1 24
#define THROTTLE_PIN_2 25

#define MESSAGE_SEPARATOR ';'

void setup()
{
  pinMode(STEER_PIN_1, OUTPUT);
  pinMode(STEER_PIN_2, OUTPUT);
}

void loop()
{
  digitalWrite(STEER_PIN_1, HIGH);
  digitalWrite(STEER_PIN_2, LOW);
  delay(1000);
  digitalWrite(STEER_PIN_1, LOW);
  digitalWrite(STEER_PIN_2, HIGH);
  delay(1000);
}
