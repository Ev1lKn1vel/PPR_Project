import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Arrays;
import java.util.stream.Collectors;

public class Main {

    public static char[] charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".toCharArray();
    private static String HASHTYPE = "SHA-1";
    private static final String charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static void main(String[] args) throws NoSuchAlgorithmException {

        String password = "ZZZ";

        MessageDigest digest = MessageDigest.getInstance(HASHTYPE);
        byte[] hashedPassword = digest.digest(password.getBytes());

        long start = System.currentTimeMillis();
//        String guessedPW = guessPasswordSerial(hashedPassword);
//        String guessedPW = guessPasswordParallel_fixedLength(hashedPassword);
        String guessedPW = guessPasswordParellel(hashedPassword, 5, "");
//        String guessedPW = guessPasswordSerial(hashedPassword, password.length(), "");

        long end = System.currentTimeMillis();
        System.out.println("total Time: " + (end - start) + "ms and password is " + guessedPW);

    }

    public static String guessPasswordSerial_fixedLength(byte[] hashedPassword) throws NoSuchAlgorithmException {
        MessageDigest digest = MessageDigest.getInstance(HASHTYPE);

        for (char first : charList) {
            for (char second : charList) {
                for (char third : charList) {
                    for (char fourth : charList) {
                        String possiblePW = "" + first + second + third + fourth;
                        byte[] possibleSolution = digest.digest(possiblePW.getBytes());
                        if (Arrays.equals(hashedPassword, possibleSolution)) {
                            return possiblePW;
                        }
                    }
                }
            }
        }
        return "Password not found";
    }

    public static String guessPasswordSerial(byte[] hashedPassword, int maxLength, String guessedPw) throws NoSuchAlgorithmException {
        //in most inner loop
        if (maxLength == 0) {
            MessageDigest digest = MessageDigest.getInstance(HASHTYPE);
            byte[] possibleSolution = digest.digest(guessedPw.getBytes());
            if (Arrays.equals(hashedPassword, possibleSolution)) {
                return guessedPw;
            }
        } else {
            for (char c : charList) {
                String tmp = guessPasswordSerial(hashedPassword, maxLength - 1, guessedPw + c);
                if (tmp != null && !tmp.isBlank()) {
                    return tmp;
                }
            }
        }


        return "";
    }

    public static String guessPasswordParallel_fixedLength(byte[] hashedPassword) {
        return charArray.chars().parallel().mapToObj(first -> {
            for (char second : charList) {
                for (char third : charList) {
                    for (char fourth : charList) {
                        String possiblePW = "" + (char) first + second + third + fourth;

                        try {
                            MessageDigest digestInsied = MessageDigest.getInstance(HASHTYPE);
                            byte[] possibleSolution = digestInsied.digest(possiblePW.getBytes());
                            if (Arrays.equals(hashedPassword, possibleSolution)) {
                                return possiblePW;
                            }

                        } catch (Exception e) {
                            return "";
                        }
                    }
                }
            }
            return "";
        })
                .filter(item -> item != null && !item.isEmpty())
                .collect(Collectors.joining());
    }

    public static String guessPasswordParellel_fixedLength(byte[] hashedPassword, int maxLength, String guessedPw) {

        return charArray.chars().parallel().mapToObj(item -> recursiveLoop(maxLength - 1, (char) item + "", hashedPassword))
                .filter(item -> item != null && !item.isBlank())
                .collect(Collectors.joining());
    }


    private static String recursiveLoop_fixedLength(int maxLength, String guessedPw, byte[] hashedPassword) {
        if (maxLength == 0) {
            try {
                MessageDigest digestInside = MessageDigest.getInstance(HASHTYPE);
                byte[] possibleSolution = digestInside.digest(guessedPw.getBytes());
                if (Arrays.equals(hashedPassword, possibleSolution)) {
                    return guessedPw;
                }

            } catch (Exception e) {
                return null;
            }
        } else {
            for (char c : charList) {
                String tmp = recursiveLoop(maxLength - 1, guessedPw + c, hashedPassword);
                if (tmp != null && !tmp.isBlank()) {
                    return tmp;
                }
            }
        }
        return null;
    }

    public static String guessPasswordParellel(byte[] hashedPassword, int maxLength, String guessedPw) {

        return charArray.chars().parallel().mapToObj(item -> recursiveLoop(maxLength - 1, (char) item + "", hashedPassword))
                .filter(item -> item != null && !item.isBlank())
                .collect(Collectors.joining());
    }


    private static String recursiveLoop(int maxLength, String guessedPw, byte[] hashedPassword) {

        try {
            MessageDigest digestInside = MessageDigest.getInstance(HASHTYPE);
            byte[] possibleSolution = digestInside.digest(guessedPw.getBytes());
            if (Arrays.equals(hashedPassword, possibleSolution)) {
                return guessedPw;
            }

        } catch (Exception e) {
            return null;
        }
        if (maxLength > 0) {

            for (char c : charList) {
                String tmp = recursiveLoop(maxLength - 1, guessedPw + c, hashedPassword);
                if (tmp != null && !tmp.isBlank()) {
                    return tmp;
                }
            }
        }
        return null;
    }


}
