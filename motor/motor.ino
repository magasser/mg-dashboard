#include <Message.h>
#include <Controller.h>
#include <Engine.h>
#include <SerialConnection.h>

#define LOOP_DELAY 100

#define SERIAL_BAUD_RATE 9600
#define SERIAL_TIMEOUT 100

#define STEER_PIN_1 22
#define STEER_PIN_2 23
#define THROTTLE_PIN_1 24
#define THROTTLE_PIN_2 25

Engine engine(STEER_PIN_1, STEER_PIN_2, THROTTLE_PIN_1, THROTTLE_PIN_2);
Controller controller(engine);
SerialConnection connection(Serial, SERIAL_BAUD_RATE, SERIAL_TIMEOUT);

void setup()
{
  controller.init();
  connection.init();

  connection.connect();
}

void loop()
{
  delay(LOOP_DELAY);
}

void serialEvent() {
  if (connection.dataAvailable()) {
    String value = connection.readUntil(MESSAGE_SEPARATOR);
    Message message(value);

    if (message.type.equals(CON_MESSAGE)) {
      sendAckMessage(CON_MESSAGE);
    } else if (message.type.equals(CMD_MESSAGE)) {
      executeCommand(message);
    } else if (message.type.equals(MSG_MESSAGE)) {
      // TODO
    } else if (message.type.equals(MOD_MESSAGE)) {
      // TODO
    } else {
      // Do nothing
    }
  }
}

void executeCommand(const Message& command) {  
  if (controller.execute(command.parameters, command.parameterCount)) {
    String message = command.type;
    for (int i = 0; i < command.parameterCount; i++) {
      message += String(PARAMETER_SEPARATOR) + command.parameters[i];
    }
    sendAckMessage(message);
  }
}

// TODO: improve this
void sendAckMessage(String message) {
  String ackMessage = String(ACK_MESSAGE) + TYPE_SEPARATOR + message + MESSAGE_SEPARATOR;
  connection.write(ackMessage);
}
