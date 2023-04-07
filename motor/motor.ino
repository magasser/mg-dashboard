#include <SerialConnection.h>

#define SERIAL_SPEED 9600
#define SERIAL_TIMEOUT 500

SerialConnection connection(SerialUSB, SERIAL_SPEED, SERIAL_TIMEOUT);

void setup()
{
  while (!connection.connect())
  {
    delay(100);
  }
}

void loop()
{
  if (connection.available())
  {
    connection.write("hello");
  }
}
