#ifndef MESSAGE_h
#define MESSAGE_h

#include "Arduino.h"

#define MESSAGE_SEPARATOR ';'
#define TYPE_SEPARATOR '.'
#define PARAMETER_SEPARATOR ','

#define CMD_MESSAGE "cmd"
#define MOD_MESSAGE "mod"
#define MSG_MESSAGE "msg"
#define CON_MESSAGE "con"
#define ACK_MESSAGE "ack"
#define INV_MESSAGE "inv"

class Message
{
public:
    Message(const String &value);
    ~Message();
public:
    String type;
    String *parameters;
    uint8_t parameterCount;

private:
    String *splitByChar(const String &value, char separator, uint8_t size) const;
    uint8_t charCount(const String &value, char character) const;

private:
    String m_Value;
};

#endif