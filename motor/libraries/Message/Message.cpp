#include "Message.h"

Message::Message(const String &value)
{
    m_Value = value;

    uint8_t partCount = charCount(m_Value, TYPE_SEPARATOR) + 1;
    String *parts = splitByChar(m_Value, TYPE_SEPARATOR, partCount);

    type = parts[0];
    uint8_t typeLength = type.length();
    if (partCount == 1 && typeLength != 0)
    {
        parameters = new String[0];
        parameterCount = 0;
    }
    else if (partCount == 2)
    {
        parameterCount = charCount(parts[1], PARAMETER_SEPARATOR) + 1;
        parameters = splitByChar(parts[1], PARAMETER_SEPARATOR, parameterCount);
    }
    else
    {
        type = INV_MESSAGE;
        parameters = new String[0];
        parameterCount = 0;
    }
}

Message::~Message()
{
    //delete[] parameters;
}

String *Message::splitByChar(const String &value, char separator, uint8_t size) const
{
    String *splits = new String[size];

    uint16_t lastSplit = 0;
    uint16_t number = 0;

    for (uint16_t i = 0; i < value.length(); i++)
    {
        if (value.charAt(i) == separator)
        {
            splits[number++] = value.substring(lastSplit, i);
            lastSplit = i + 1;
        }
    }

    splits[number] = value.substring(lastSplit);

    return splits;
}

uint8_t Message::charCount(const String &value, char character) const
{
    uint8_t count = 0;
    for (uint16_t i = 0; i < value.length(); i++)
    {
        if (value.charAt(i) == character)
        {
            count++;
        }
    }

    return count;
}